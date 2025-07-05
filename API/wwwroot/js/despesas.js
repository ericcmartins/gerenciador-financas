import { api } from './api.js';
import { CONFIG } from './config.js';

document.addEventListener('DOMContentLoaded', () => {
    const idUsuario = localStorage.getItem('finon_user_id');
    if (!idUsuario) return;

    // --- Seleção de Elementos ---
    const btnNovaDespesa = document.getElementById('btnNovaDespesa');
    const corpoTabelaDespesas = document.getElementById('corpoTabelaDespesas');
    const filtroData = document.getElementById('filtroData');
    const filtroCategoria = document.getElementById('filtroCategoria');
    const filtroConta = document.getElementById('filtroConta');
    const filtroMetodo = document.getElementById('filtroMetodo');
    const btnAplicarFiltros = document.getElementById('btnAplicarFiltros');
    const totalDespesasElement = document.getElementById('totalDespesas');
    const despesasRecorrentesElement = document.getElementById('despesasRecorrentes');
    const mediaMensalElement = document.getElementById('mediaMensal');

    // Elementos do Modal
    const fundoModal = document.getElementById('fundoModal');
    const modalDespesa = document.getElementById('modalDespesa');
    const tituloModal = document.getElementById('tituloModal');
    const fecharModal = document.getElementById('fecharModal');
    const btnCancelarModal = document.getElementById('btnCancelarModal');
    const formularioDespesa = document.getElementById('formularioDespesa');
    const inputValor = document.getElementById('valor');
    const inputDescricao = document.getElementById('descricao');
    const inputDataDespesa = document.getElementById('dataDespesa');
    const selectCategoria = document.getElementById('categoria');
    const selectConta = document.getElementById('conta');
    const selectMetodoPagamento = document.getElementById('metodoPagamento');
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

    let despesaSendoEditadaId = null;

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
     * Abre o modal de despesas.
     */
    async function abrirModalDespesa(despesa = null) {
        fundoModal.classList.add('ativo');
        modalDespesa.classList.add('ativo');
        document.body.style.overflow = 'hidden'; // Evita rolagem do corpo

        formularioDespesa.reset();
        grupoFrequencia.style.display = 'none'; // Esconde a frequência por padrão
        checkboxRecorrente.checked = false; // Desmarca o checkbox por padrão
        despesaSendoEditadaId = null;

        // Preenche os selects
        try {
            const categorias = await api.buscarCategorias({ idUsuario });
            popularSelect(selectCategoria, categorias, 'idCategoria', 'nome', 'Selecione uma categoria');

            const contas = await api.buscarContas({ idUsuario });
            popularSelect(selectConta, contas, 'idConta', 'numeroConta', 'Selecione uma conta');

            const metodosPagamento = await api.buscarMetodosPagamento({ idUsuario });
            popularSelect(selectMetodoPagamento, metodosPagamento, 'idMetodoPagamento', 'nome', 'Selecione um método de pagamento');

        } catch (error) {
            console.error('Erro ao carregar dados para os selects do modal:', error);
            // Poderíamos adicionar uma mensagem de erro na interface aqui.
        }

        if (despesa) {
            tituloModal.textContent = 'Editar Despesa';
            inputValor.value = despesa.valor;
            inputDescricao.value = despesa.descricao;
            inputDataDespesa.value = CONFIG.UTIL.formatarDataParaInput(despesa.dataDespesa);
            // Preencher os selects com os valores da despesa
            // A API de despesas não retorna idCategoria, idConta, idMetodoPagamento.
            // Precisaríamos buscar as despesas individualmente ou ajustar a API para retornar esses IDs.
            // Por enquanto, faremos a correspondência pelo nome, o que não é ideal, mas funcional.
            // Em uma aplicação real, a API deveria retornar os IDs para facilitar.
            const categoriaSelecionada = Array.from(selectCategoria.options).find(option => option.textContent === despesa.categoria);
            if (categoriaSelecionada) selectCategoria.value = categoriaSelecionada.value;

            const contaSelecionada = Array.from(selectConta.options).find(option => option.textContent === despesa.conta);
            if (contaSelecionada) selectConta.value = contaSelecionada.value;
            
            const metodoSelecionado = Array.from(selectMetodoPagamento.options).find(option => option.textContent === despesa.metodoPagamento);
            if (metodoSelecionado) selectMetodoPagamento.value = metodoSelecionado.value;

            // Se for recorrente, ativar e preencher frequência (se a API retornar essa informação)
            // if (despesa.recorrente) {
            //     checkboxRecorrente.checked = true;
            //     grupoFrequencia.style.display = 'block';
            //     inputFrequencia.value = despesa.frequencia;
            // }
            despesaSendoEditadaId = despesa.idDespesa; // Supondo que a despesa tenha um ID
        } else {
            tituloModal.textContent = 'Nova Despesa';
            inputDataDespesa.value = CONFIG.UTIL.formatarDataParaInput(new Date().toISOString().split('T')[0]); // Data atual
        }
    }

    /**
     * Fecha o modal de despesas.
     */
    function fecharModalDespesa() {
        fundoModal.classList.remove('ativo');
        modalDespesa.classList.remove('ativo');
        document.body.style.overflow = ''; // Restaura a rolagem
        formularioDespesa.reset();
        despesaSendoEditadaId = null;
        limparErrosFormulario(formularioDespesa);
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
            exibirErroCampo(inputFrequencia, 'A frequência é obrigatória para despesas recorrentes.');
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

    async function carregarDespesas() {
        corpoTabelaDespesas.innerHTML = `<tr><td colspan="8" class="estado-carregando">Carregando despesas...</td></tr>`;
        try {
            const periodo = filtroData.value;
            const idCategoria = filtroCategoria.value;
            const idConta = filtroConta.value;
            const idMetodoPagamento = filtroMetodo.value;

            // Converter o período selecionado para o formato que a API espera (dias)
            const periodoEmDias = converterPeriodoParaDias(periodo);

            const params = {
                idUsuario: idUsuario,
                periodo: periodoEmDias // Adiciona o período em dias
            };

            // Adiciona os outros filtros se estiverem selecionados
            if (idCategoria) params.idCategoria = idCategoria;
            if (idConta) params.idConta = idConta;
            if (idMetodoPagamento) params.idMetodoPagamento = idMetodoPagamento;

            const todasAsDespesas = await api.buscarDespesas(params);

            // Atualiza o resumo
            const total = todasAsDespesas.reduce((acc, despesa) => acc + despesa.valor, 0);
            totalDespesasElement.textContent = CONFIG.UTIL.formatarMoeda(total);

            // TODO: Implementar lógica para despesas recorrentes e média mensal
            despesasRecorrentesElement.textContent = todasAsDespesas.filter(d => d.recorrente).length; // Supondo que a API retorne 'recorrente'
            const media = todasAsDespesas.length > 0 ? total / (periodo === '12' ? 12 : parseInt(periodo)) : 0; // Exemplo simples
            mediaMensalElement.textContent = CONFIG.UTIL.formatarMoeda(media);
            
            // Paginação
            totalPaginas = Math.ceil(todasAsDespesas.length / itensPorPagina);
            const inicio = (paginaAtual - 1) * itensPorPagina;
            const fim = inicio + itensPorPagina;
            const despesasPaginadas = todasAsDespesas.slice(inicio, fim);

            corpoTabelaDespesas.innerHTML = ''; // Limpa antes de preencher

            if (despesasPaginadas.length === 0) {
                corpoTabelaDespesas.innerHTML = `<tr><td colspan="8" class="estado-vazio">Nenhuma despesa encontrada com os filtros aplicados.</td></tr>`;
                infoPagina.textContent = 'Página 0 de 0';
                btnPaginaAnterior.disabled = true;
                btnProximaPagina.disabled = true;
                return;
            }

            despesasPaginadas.forEach(despesa => {
                const row = corpoTabelaDespesas.insertRow();
                row.innerHTML = `
                    <td>${CONFIG.UTIL.formatarData(despesa.dataDespesa)}</td>
                    <td>${despesa.descricao}</td>
                    <td>${despesa.categoria || 'N/A'}</td>
                    <td>${despesa.conta || 'N/A'}</td>
                    <td>${despesa.metodoPagamento || 'N/A'}</td>
                    <td class="valor-despesa">${CONFIG.UTIL.formatarMoeda(despesa.valor)}</td>
                    <td>${despesa.recorrente ? '<i class="fas fa-check-circle texto-sucesso"></i> Sim' : '<i class="fas fa-times-circle texto-erro"></i> Não'}</td>
                    <td class="acoes">
                        <button class="botao-icone" data-action="editar" data-id="${despesa.idDespesa}"><i class="fas fa-edit"></i></button>
                        <button class="botao-icone" data-action="excluir" data-id="${despesa.idDespesa}"><i class="fas fa-trash-alt"></i></button>
                    </td>
                `;
            });

            atualizarControlesPaginacao();

        } catch (error) {
            console.error('Erro ao carregar despesas:', error.message);
            corpoTabelaDespesas.innerHTML = `<tr><td colspan="8" class="estado-erro">Erro ao carregar despesas.</td></tr>`;
            exibirNotificacao('Erro ao carregar despesas.', 'erro');
        }
    }

    async function popularFiltros() {
        try {
            const categorias = await api.buscarCategorias({ idUsuario });
            popularSelect(filtroCategoria, categorias, 'idCategoria', 'nome', 'Todas as categorias');

            const contas = await api.buscarContas({ idUsuario });
            popularSelect(filtroConta, contas, 'idConta', 'numeroConta', 'Todas as contas');

            const metodosPagamento = await api.buscarMetodosPagamento({ idUsuario });
            popularSelect(filtroMetodo, metodosPagamento, 'idMetodoPagamento', 'nome', 'Todos os métodos');
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
            carregarDespesas();
        }
    }

    // --- Manipuladores de Eventos ---

    btnNovaDespesa.addEventListener('click', () => abrirModalDespesa());
    fecharModal.addEventListener('click', fecharModalDespesa);
    btnCancelarModal.addEventListener('click', fecharModalDespesa);

    // Fechar modal ao clicar fora
    fundoModal.addEventListener('click', (event) => {
        if (event.target === fundoModal) {
            fecharModalDespesa();
        }
    });

    // Lógica para mostrar/esconder campo de frequência
    checkboxRecorrente.addEventListener('change', () => {
        grupoFrequencia.style.display = checkboxRecorrente.checked ? 'block' : 'none';
        inputFrequencia.required = checkboxRecorrente.checked; // Torna obrigatório se marcado
    });

    // Submissão do formulário de despesa
    formularioDespesa.addEventListener('submit', async (event) => {
        event.preventDefault();
        
        if (!validarFormulario(formularioDespesa)) {
            exibirNotificacao('Por favor, preencha todos os campos obrigatórios corretamente.', 'erro');
            return;
        }

        const dadosDespesa = {
            valor: parseFloat(inputValor.value),
            descricao: inputDescricao.value,
            dataDespesa: inputDataDespesa.value, // A API espera 'yyyy-MM-dd' ou similar
            // A API de despesas não tem campo 'recorrente' no momento, assumindo que será adicionado
            // recorrente: checkboxRecorrente.checked, 
            // frequencia: checkboxRecorrente.checked ? parseInt(inputFrequencia.value) : null
        };

        const params = {
            idUsuario: idUsuario,
            idCategoria: parseInt(selectCategoria.value),
            idConta: parseInt(selectConta.value),
            idMetodoPagamento: parseInt(selectMetodoPagamento.value)
        };

        try {
            if (despesaSendoEditadaId) {
                // Atualizar Despesa
                params.idDespesa = despesaSendoEditadaId;
                await api.atualizarDespesa(dadosDespesa, params);
                exibirNotificacao('Despesa atualizada com sucesso!', 'sucesso');
            } else {
                // Criar Nova Despesa
                await api.criarDespesa(dadosDespesa, params);
                exibirNotificacao('Despesa criada com sucesso!', 'sucesso');
            }
            fecharModalDespesa();
            carregarDespesas(); // Recarrega a lista após a operação
        } catch (error) {
            console.error('Erro ao salvar despesa:', error.message);
            exibirNotificacao('Erro ao salvar despesa. Verifique os dados.', 'erro');
        }
    });

    // Ações da tabela (editar/excluir)
    corpoTabelaDespesas.addEventListener('click', async (event) => {
        const target = event.target.closest('button');
        if (!target) return;

        const action = target.dataset.action;
        const idDespesa = target.dataset.id; // Supondo que a despesa tenha um ID

        if (action === 'editar') {
            try {
                // Para editar, precisamos dos dados completos da despesa.
                // A API de buscarDespesas retorna DespesaResponseViewModel.
                // Precisamos filtrar a despesa específica pelo ID.
                const despesasAtuais = await api.buscarDespesas({ idUsuario, periodo: converterPeriodoParaDias(filtroData.value) });
                const despesaParaEditar = despesasAtuais.find(d => d.idDespesa == idDespesa); // Comparação solta para garantir

                if (despesaParaEditar) {
                    abrirModalDespesa(despesaParaEditar);
                } else {
                    exibirNotificacao('Despesa não encontrada para edição.', 'erro');
                }
            } catch (error) {
                console.error('Erro ao buscar despesa para edição:', error.message);
                exibirNotificacao('Erro ao carregar dados da despesa para edição.', 'erro');
            }
        } else if (action === 'excluir') {
            if (confirm('Tem certeza que deseja excluir esta despesa?')) {
                try {
                    await api.deletarDespesa({ idUsuario, idDespesa });
                    exibirNotificacao('Despesa excluída com sucesso!', 'sucesso');
                    carregarDespesas(); // Recarrega a lista após a exclusão
                } catch (error) {
                    console.error('Erro ao excluir despesa:', error.message);
                    exibirNotificacao('Erro ao excluir despesa.', 'erro');
                }
            }
        }
    });

    // Filtros
    btnAplicarFiltros.addEventListener('click', () => {
        paginaAtual = 1; // Volta para a primeira página ao aplicar filtros
        carregarDespesas();
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
    carregarDespesas(); // Carrega as despesas iniciais
});

