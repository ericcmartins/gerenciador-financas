import { api } from './api.js';
import { CONFIG } from './config.js';

document.addEventListener('DOMContentLoaded', () => {
    const idUsuario = localStorage.getItem('finon_user_id');
    if (!idUsuario) return;

    // --- Seleção de Elementos ---
    const btnNovaReceita = document.getElementById('btnNovaReceita');
    const corpoTabelaReceitas = document.getElementById('corpoTabelaReceitas');
    const filtroData = document.getElementById('filtroData');
    const filtroCategoria = document.getElementById('filtroCategoria');
    const filtroConta = document.getElementById('filtroConta');
    const btnAplicarFiltros = document.getElementById('btnAplicarFiltros');
    const totalReceitasElement = document.getElementById('totalReceitas');
    const receitasRecorrentesElement = document.getElementById('receitasRecorrentes');
    const mediaMensalElement = document.getElementById('mediaMensal');

    // Elementos do Modal
    const fundoModal = document.getElementById('fundoModal');
    const modalReceita = document.getElementById('modalReceita');
    const tituloModal = document.getElementById('tituloModal');
    const fecharModal = document.getElementById('fecharModal');
    const btnCancelarModal = document.getElementById('btnCancelar'); // ID atualizado para 'btnCancelar'
    const formularioReceita = document.getElementById('formularioReceita');
    const inputReceitaId = document.getElementById('receitaId'); // Campo hidden para o ID da receita
    const inputValor = document.getElementById('valor');
    const inputDescricao = document.getElementById('descricao');
    const inputDataReceita = document.getElementById('dataReceita');
    const selectCategoria = document.getElementById('categoria');
    const selectConta = document.getElementById('conta');
    const checkboxRecorrente = document.getElementById('recorrente');
    const grupoFrequencia = document.getElementById('grupoFrequencia');
    const inputFrequencia = document.getElementById('frequencia');

    // Variáveis de Paginação
    let paginaAtual = 1;
    const itensPorPagina = CONFIG.APP.ITENS_POR_PAGINA;
    let totalPaginas = 1;
    const infoPagina = document.getElementById('infoPagina');
    const btnPaginaAnterior = document.getElementById('paginaAnterior');
    const btnProximaPagina = document.getElementById('proximaPagina');

    // --- Funções Auxiliares ---

    /**
     * Converte o valor do seletor de período para o número de dias correspondente.
     * @param {string} valorPeriodo - O valor da opção selecionada ('1', '3', '6', '12').
     * @returns {number} O número de dias.
     */
    function converterPeriodoParaDias(valorPeriodo) {
        const hoje = new Date();
        switch (valorPeriodo) {
            case '1': // Este Mês
                return hoje.getDate(); // Retorna o dia do mês, que é o número de dias corridos.
            case '3': // Últimos 3 Meses
                return 90; // Aproximação
            case '6': // Últimos 6 Meses
                return 180; // Aproximação
            case '12': // Este Ano
                const inicioDoAno = new Date(hoje.getFullYear(), 0, 1);
                const diffEmMs = hoje - inicioDoAno;
                const diffEmDias = Math.ceil(diffEmMs / (1000 * 60 * 60 * 24));
                return diffEmDias;
            default:
                return 30; // Padrão para 30 dias se algo der errado.
        }
    }

    /**
     * Popula um elemento <select> com opções.
     * @param {HTMLElement} selectElement - O elemento <select> a ser populado.
     * @param {Array} data - Os dados para preencher as opções.
     * @param {string} valueKey - A chave do objeto a ser usada como valor da opção.
     * @param {string} textKey - A chave do objeto a ser usada como texto da opção.
     * @param {string} placeholder - O texto do placeholder inicial.
     */
    function popularSelect(selectElement, data, valueKey, textKey, placeholder) {
        selectElement.innerHTML = `<option value="">${placeholder}</option>`;
        data.forEach(item => {
            const option = document.createElement('option');
            option.value = item[valueKey];
            option.textContent = item[textKey];
            selectElement.appendChild(option);
        });
    }

    /**
     * Abre o modal de receitas.
     */
    async function abrirModalReceita(receita = null) {
        fundoModal.classList.add('ativo');
        modalReceita.classList.add('ativo');
        document.body.style.overflow = 'hidden'; // Evita rolagem do corpo

        formularioReceita.reset();
        grupoFrequencia.style.display = 'none'; // Esconde a frequência por padrão
        checkboxRecorrente.checked = false; // Desmarca o checkbox por padrão
        inputReceitaId.value = ''; // Limpa o ID da receita sendo editada

        // Preenche os selects
        try {
            const categorias = await api.buscarCategorias({ idUsuario });
            popularSelect(selectCategoria, categorias, 'idCategoria', 'nome', 'Selecione uma categoria');

            const contas = await api.buscarContas({ idUsuario });
            popularSelect(selectConta, contas, 'idConta', 'numeroConta', 'Selecione uma conta');

        } catch (error) {
            console.error('Erro ao carregar dados para os selects do modal:', error.message);
            // Poderíamos adicionar uma mensagem de erro na interface aqui.
        }

        if (receita) {
            tituloModal.textContent = 'Editar Receita';
            inputReceitaId.value = receita.idReceita; // Define o ID da receita para edição
            inputValor.value = receita.valor;
            inputDescricao.value = receita.descricao;
            inputDataReceita.value = CONFIG.UTIL.formatarDataParaInput(receita.dataReceita);
            
            // Preencher os selects com os valores da receita
            // A API de receitas não retorna idCategoria, idConta diretamente no ReceitaResponseViewModel.
            // Precisaríamos buscar as receitas individualmente ou ajustar a API para retornar esses IDs.
            // Por enquanto, faremos a correspondência pelo nome.
            const categoriaSelecionada = Array.from(selectCategoria.options).find(option => option.textContent === receita.categoria);
            if (categoriaSelecionada) selectCategoria.value = categoriaSelecionada.value;

            const contaSelecionada = Array.from(selectConta.options).find(option => option.textContent === receita.conta);
            if (contaSelecionada) selectConta.value = contaSelecionada.value;
            
            // Lógica para receitas recorrentes (se a API suportar e retornar o campo 'recorrente' e 'frequencia')
            // if (receita.recorrente) {
            //     checkboxRecorrente.checked = true;
            //     grupoFrequencia.style.display = 'block';
            //     inputFrequencia.value = receita.frequencia;
            // }
        } else {
            tituloModal.textContent = 'Nova Receita';
            inputDataReceita.value = CONFIG.UTIL.formatarDataParaInput(new Date().toISOString().split('T')[0]); // Data atual
        }
    }

    /**
     * Fecha o modal de receitas.
     */
    function fecharModalReceita() {
        fundoModal.classList.remove('ativo');
        modalReceita.classList.remove('ativo');
        document.body.style.overflow = ''; // Restaura a rolagem
        formularioReceita.reset();
        inputReceitaId.value = '';
        limparErrosFormulario(formularioReceita);
    }

    /**
     * Valida o formulário.
     * @param {HTMLFormElement} form - O formulário a ser validado.
     * @returns {boolean} True se o formulário for válido, false caso contrário.
     */
    function validarFormulario(form) {
        limparErrosFormulario(form);
        let isValid = true;
        const camposObrigatorios = form.querySelectorAll('[required]');

        camposObrigatorios.forEach(campo => {
            if (!campo.value.trim()) {
                exibirErroCampo(campo, 'Este campo é obrigatório.');
                isValid = false;
            }
        });

        // Validação específica para campos numéricos
        const camposNumericos = form.querySelectorAll('input[type="number"]');
        camposNumericos.forEach(campo => {
            if (campo.value && isNaN(parseFloat(campo.value))) {
                exibirErroCampo(campo, 'Deve ser um número válido.');
                isValid = false;
            }
            if (campo.hasAttribute('min') && parseFloat(campo.value) < parseFloat(campo.min)) {
                exibirErroCampo(campo, `O valor mínimo é ${campo.min}.`);
                isValid = false;
            }
        });
        
        // Validação de selects
        const selectsObrigatorios = form.querySelectorAll('select[required]');
        selectsObrigatorios.forEach(select => {
            if (!select.value) {
                exibirErroCampo(select, 'Selecione uma opção.');
                isValid = false;
            }
        });

        if (checkboxRecorrente.checked && !inputFrequencia.value.trim()) {
            exibirErroCampo(inputFrequencia, 'A frequência é obrigatória para receitas recorrentes.');
            isValid = false;
        }

        return isValid;
    }

    /**
     * Exibe uma mensagem de erro abaixo de um campo do formulário.
     * @param {HTMLElement} campo - O campo onde o erro ocorreu.
     * @param {string} mensagem - A mensagem de erro.
     */
    function exibirErroCampo(campo, mensagem) {
        campo.classList.add('campo-erro');
        let erroElement = campo.nextElementSibling;
        if (!erroElement || !erroElement.classList.contains('mensagem-erro')) {
            erroElement = document.createElement('span');
            erroElement.classList.add('mensagem-erro');
            campo.parentNode.insertBefore(erroElement, campo.nextSibling);
        }
        erroElement.textContent = mensagem;
    }

    /**
     * Limpa as mensagens de erro de um formulário.
     * @param {HTMLFormElement} form - O formulário a ser limpo.
     */
    function limparErrosFormulario(form) {
        form.querySelectorAll('.campo-erro').forEach(campo => {
            campo.classList.remove('campo-erro');
        });
        form.querySelectorAll('.mensagem-erro').forEach(erro => {
            erro.remove();
        });
    }

    /**
     * Exibe uma notificação global (sucesso ou erro).
     * @param {string} mensagem - A mensagem a ser exibida.
     * @param {string} tipo - 'sucesso' ou 'erro'.
     */
    function exibirNotificacao(mensagem, tipo) {
        const notificacaoDiv = document.createElement('div');
        notificacaoDiv.classList.add('notificacao', tipo);
        notificacaoDiv.textContent = mensagem;
        document.body.appendChild(notificacaoDiv);

        setTimeout(() => {
            notificacaoDiv.remove();
        }, 3000);
    }

    // --- Funções de Carregamento de Dados e Tabela ---

    async function carregarReceitas() {
        corpoTabelaReceitas.innerHTML = `<tr><td colspan="7" class="estado-carregando">Carregando receitas...</td></tr>`;
        try {
            const periodo = filtroData.value;
            const idCategoria = filtroCategoria.value;
            const idConta = filtroConta.value;

            // Converter o período selecionado para o formato que a API espera (dias)
            const periodoEmDias = converterPeriodoParaDias(periodo);

            const params = {
                idUsuario: idUsuario,
                periodo: periodoEmDias // Adiciona o período em dias
            };

            // Adiciona os outros filtros se estiverem selecionados
            if (idCategoria) params.idCategoria = idCategoria;
            if (idConta) params.idConta = idConta;

            const todasAsReceitas = await api.buscarReceitas(params);

            // Atualiza o resumo
            const total = todasAsReceitas.reduce((acc, receita) => acc + receita.valor, 0);
            totalReceitasElement.textContent = CONFIG.UTIL.formatarMoeda(total);

            // TODO: A API atual não retorna 'recorrente'.
            // Assumindo que o campo 'recorrente' será adicionado ao ReceitaResponseViewModel
            // const receitasRecorrentes = todasAsReceitas.filter(r => r.recorrente).length; 
            // receitasRecorrentesElement.textContent = receitasRecorrentes;
            receitasRecorrentesElement.textContent = 'N/A'; // Temporário

            const media = todasAsReceitas.length > 0 ? total / (periodo === '12' ? 12 : parseInt(periodo)) : 0; // Exemplo simples
            mediaMensalElement.textContent = CONFIG.UTIL.formatarMoeda(media);
            
            // Paginação
            totalPaginas = Math.ceil(todasAsReceitas.length / itensPorPagina);
            const inicio = (paginaAtual - 1) * itensPorPagina;
            const fim = inicio + itensPorPagina;
            const receitasPaginadas = todasAsReceitas.slice(inicio, fim);

            corpoTabelaReceitas.innerHTML = ''; // Limpa antes de preencher

            if (receitasPaginadas.length === 0) {
                corpoTabelaReceitas.innerHTML = `<tr><td colspan="7" class="estado-vazio">Nenhuma receita encontrada com os filtros aplicados.</td></tr>`;
                infoPagina.textContent = 'Página 0 de 0';
                btnPaginaAnterior.disabled = true;
                btnProximaPagina.disabled = true;
                return;
            }

            receitasPaginadas.forEach(receita => {
                const row = corpoTabelaReceitas.insertRow();
                // Assumindo que a API retornará 'idReceita' para ações
                row.innerHTML = `
                    <td>${CONFIG.UTIL.formatarData(receita.dataReceita)}</td>
                    <td>${receita.descricao}</td>
                    <td>${receita.categoria || 'N/A'}</td>
                    <td>${receita.conta || 'N/A'}</td>
                    <td class="valor-receita">${CONFIG.UTIL.formatarMoeda(receita.valor)}</td>
                    <td>${receita.recorrente ? '<i class="fas fa-check-circle texto-sucesso"></i> Sim' : '<i class="fas fa-times-circle texto-erro"></i> Não'}</td>
                    <td class="acoes">
                        <button class="botao-icone" data-action="editar" data-id="${receita.idReceita}"><i class="fas fa-edit"></i></button>
                        <button class="botao-icone" data-action="excluir" data-id="${receita.idReceita}"><i class="fas fa-trash-alt"></i></button>
                    </td>
                `;
            });

            atualizarControlesPaginacao();

        } catch (error) {
            console.error('Erro ao carregar receitas:', error.message);
            corpoTabelaReceitas.innerHTML = `<tr><td colspan="7" class="estado-erro">Erro ao carregar receitas.</td></tr>`;
            exibirNotificacao('Erro ao carregar receitas.', 'erro');
        }
    }

    async function popularFiltros() {
        try {
            const categorias = await api.buscarCategorias({ idUsuario });
            popularSelect(filtroCategoria, categorias, 'idCategoria', 'nome', 'Todas as categorias');

            const contas = await api.buscarContas({ idUsuario });
            popularSelect(filtroConta, contas, 'idConta', 'numeroConta', 'Todas as contas');
        } catch (error) {
            console.error('Erro ao popular filtros:', error.message);
        }
    }

    // --- Funções de Paginação ---
    function atualizarControlesPaginacao() {
        infoPagina.textContent = `Página ${paginaAtual} de ${totalPaginas}`;
        btnPaginaAnterior.disabled = paginaAtual === 1;
        btnProximaPagina.disabled = paginaAtual === totalPaginas;
    }

    function irParaPagina(novaPagina) {
        if (novaPagina >= 1 && novaPagina <= totalPaginas) {
            paginaAtual = novaPagina;
            carregarReceitas();
        }
    }

    // --- Manipuladores de Eventos ---

    btnNovaReceita.addEventListener('click', () => abrirModalReceita());
    fecharModal.addEventListener('click', fecharModalReceita);
    btnCancelarModal.addEventListener('click', fecharModalReceita);

    // Fechar modal ao clicar fora
    fundoModal.addEventListener('click', (event) => {
        if (event.target === fundoModal) {
            fecharModalReceita();
        }
    });

    // Lógica para mostrar/esconder campo de frequência
    checkboxRecorrente.addEventListener('change', () => {
        grupoFrequencia.style.display = checkboxRecorrente.checked ? 'block' : 'none';
        inputFrequencia.required = checkboxRecorrente.checked; // Torna obrigatório se marcado
    });

    // Submissão do formulário de receita
    formularioReceita.addEventListener('submit', async (event) => {
        event.preventDefault();
        
        if (!validarFormulario(formularioReceita)) {
            exibirNotificacao('Por favor, preencha todos os campos obrigatórios corretamente.', 'erro');
            return;
        }

        const dadosReceita = {
            valor: parseFloat(inputValor.value),
            descricao: inputDescricao.value,
            dataReceita: inputDataReceita.value, // A API espera 'yyyy-MM-dd' ou similar
            // A API de receitas não tem campo 'recorrente' no momento, assumindo que será adicionado
            // recorrente: checkboxRecorrente.checked, 
            // frequencia: checkboxRecorrente.checked ? parseInt(inputFrequencia.value) : null
        };

        const params = {
            idUsuario: idUsuario,
            idCategoria: parseInt(selectCategoria.value),
            idConta: parseInt(selectConta.value)
        };

        try {
            const receitaId = inputReceitaId.value;
            if (receitaId) {
                // Atualizar Receita
                params.idReceita = parseInt(receitaId);
                await api.atualizarReceita(dadosReceita, params);
                exibirNotificacao('Receita atualizada com sucesso!', 'sucesso');
            } else {
                // Criar Nova Receita
                await api.criarReceita(dadosReceita, params);
                exibirNotificacao('Receita criada com sucesso!', 'sucesso');
            }
            fecharModalReceita();
            carregarReceitas(); // Recarrega a lista após a operação
        } catch (error) {
            console.error('Erro ao salvar receita:', error.message);
            exibirNotificacao('Erro ao salvar receita. Verifique os dados.', 'erro');
        }
    });

    // Ações da tabela (editar/excluir)
    corpoTabelaReceitas.addEventListener('click', async (event) => {
        const target = event.target.closest('button');
        if (!target) return;

        const action = target.dataset.action;
        const idReceita = target.dataset.id; // ID da receita vindo do data-id

        if (action === 'editar') {
            try {
                // Para editar, precisamos dos dados completos da receita.
                // A API de buscarReceitas retorna ReceitaResponseViewModel.
                // Precisamos filtrar a receita específica pelo ID.
                const receitasAtuais = await api.buscarReceitas({ idUsuario, periodo: converterPeriodoParaDias(filtroData.value) });
                const receitaParaEditar = receitasAtuais.find(r => r.idReceita == idReceita); // Comparação solta para garantir

                if (receitaParaEditar) {
                    abrirModalReceita(receitaParaEditar);
                } else {
                    exibirNotificacao('Receita não encontrada para edição.', 'erro');
                }
            } catch (error) {
                console.error('Erro ao buscar receita para edição:', error.message);
                exibirNotificacao('Erro ao carregar dados da receita para edição.', 'erro');
            }
        } else if (action === 'excluir') {
            if (confirm('Tem certeza que deseja excluir esta receita?')) {
                try {
                    await api.deletarReceita({ idUsuario, idReceita });
                    exibirNotificacao('Receita excluída com sucesso!', 'sucesso');
                    carregarReceitas(); // Recarrega a lista após a exclusão
                } catch (error) {
                    console.error('Erro ao excluir receita:', error.message);
                    exibirNotificacao('Erro ao excluir receita.', 'erro');
                }
            }
        }
    });

    // Filtros
    btnAplicarFiltros.addEventListener('click', () => {
        paginaAtual = 1; // Volta para a primeira página ao aplicar filtros
        carregarReceitas();
    });

    // Paginação
    btnPaginaAnterior.addEventListener('click', () => irParaPagina(paginaAtual - 1));
    btnProximaPagina.addEventListener('click', () => irParaPagina(paginaAtual + 1));

    // Funcionalidade de Exportar (apenas um placeholder por enquanto)
    document.getElementById('btnExportar').addEventListener('click', () => {
        exibirNotificacao('Funcionalidade de exportar ainda não implementada.', 'info');
    });

    // --- Inicialização ---
    popularFiltros();
    carregarReceitas(); // Carrega as receitas iniciais
});
