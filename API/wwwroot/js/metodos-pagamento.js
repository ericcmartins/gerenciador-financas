import { api } from './api.js';
import { CONFIG } from './config.js';

document.addEventListener('DOMContentLoaded', () => {
    const idUsuario = localStorage.getItem('finon_user_id');
    if (!idUsuario) return;

    // --- Seleção de Elementos ---
    const btnNovoMetodo = document.getElementById('btnNovoMetodo');
    const gridMetodosPagamento = document.getElementById('gridMetodosPagamento');
    const corpoTabelaMetodos = document.getElementById('corpoTabelaMetodos');
    const inputBusca = document.getElementById('inputBusca');

    // Elementos do Modal de Método de Pagamento (Adicionar/Editar)
    const fundoModalMetodo = document.getElementById('fundoModalMetodo');
    const modalMetodo = document.getElementById('modalMetodo');
    const tituloModalMetodo = modalMetodo.querySelector('#tituloModal'); // Reutiliza o ID 'tituloModal'
    const fecharModalMetodoBtn = document.getElementById('fecharModalMetodo');
    const btnCancelarMetodo = document.getElementById('btnCancelarMetodo');
    const formularioMetodo = document.getElementById('formularioMetodo');
    const inputMetodoId = document.getElementById('metodoId'); // Campo hidden para o ID do método
    const inputNomeMetodo = document.getElementById('nomeMetodo');
    const selectTipoMetodo = document.getElementById('tipoMetodo');
    const selectContaVinculada = document.getElementById('contaVinculada');
    const inputLimiteMetodo = document.getElementById('limiteMetodo');
    const inputDescricaoMetodo = document.getElementById('descricaoMetodo');

    // Elementos do Modal de Deleção
    const fundoModalDelecao = document.getElementById('fundoModalDelecao');
    const fecharModalDelecaoBtn = document.getElementById('fecharModalDelecao');
    const btnCancelarDelecao = document.getElementById('btnCancelarDelecao');
    const btnConfirmarDelecao = document.getElementById('btnConfirmarDelecao');

    let metodoSendoDeletadoId = null; // Variável para armazenar o ID do método a ser deletado
    let contasDisponiveis = []; // Armazena as contas para popular o select

    // --- Funções Auxiliares (reutilizadas de outros módulos) ---

    /**
     * Valida o formulário de método de pagamento.
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
     * Abre o modal de método de pagamento (para adicionar ou editar).
     * @param {object | null} metodo - Objeto método se for edição, null se for nova.
     */
    async function abrirModalMetodo(metodo = null) {
        fundoModalMetodo.classList.add('ativo');
        modalMetodo.classList.add('ativo');
        document.body.style.overflow = 'hidden'; // Evita rolagem do corpo

        formularioMetodo.reset();
        limparErrosFormulario(formularioMetodo);
        inputMetodoId.value = ''; // Limpa o ID para novo método

        // Popula o select de contas vinculadas
        try {
            contasDisponiveis = await api.buscarContas({ idUsuario });
            popularSelect(selectContaVinculada, contasDisponiveis, 'idConta', 'numeroConta', 'Selecione uma conta');
        } catch (error) {
            console.error('Erro ao carregar contas para o modal de método de pagamento:', error.message);
            exibirNotificacao('Erro ao carregar contas para o formulário.', 'erro');
        }

        if (metodo) {
            tituloModalMetodo.textContent = 'Editar Método de Pagamento';
            inputMetodoId.value = metodo.idMetodoPagamento; // Supondo que a API retorne idMetodoPagamento
            inputNomeMetodo.value = metodo.nome;
            selectTipoMetodo.value = metodo.tipo;
            inputLimiteMetodo.value = metodo.limite || '';
            inputDescricaoMetodo.value = metodo.descricao || '';

            // Preencher a conta vinculada se a API retornar o ID da conta no MetodoPagamentoResponseViewModel
            // ATENÇÃO: A API atual (Swagger YAML) não retorna idConta no MetodoPagamentoResponseViewModel.
            // Para que este campo seja preenchido automaticamente, a API precisaria ser ajustada.
            // Por enquanto, o select será populado, mas não pré-selecionado automaticamente.
            // Se a API for atualizada para incluir 'idConta' no response:
            // if (metodo.idConta) {
            //     selectContaVinculada.value = metodo.idConta;
            // }
        } else {
            tituloModalMetodo.textContent = 'Novo Método de Pagamento';
        }
    }

    /**
     * Fecha o modal de método de pagamento.
     */
    function fecharModalMetodo() {
        fundoModalMetodo.classList.remove('ativo');
        modalMetodo.classList.remove('ativo');
        document.body.style.overflow = ''; // Restaura a rolagem
        formularioMetodo.reset();
        limparErrosFormulario(formularioMetodo);
    }

    /**
     * Abre o modal de confirmação de deleção.
     * @param {number} id - O ID do método a ser deletado.
     */
    function abrirModalDelecao(id) {
        metodoSendoDeletadoId = id;
        fundoModalDelecao.classList.add('ativo');
        fundoModalDelecao.querySelector('.janela-modal').classList.add('ativo');
        document.body.style.overflow = 'hidden';
    }

    /**
     * Fecha o modal de confirmação de deleção.
     */
    function fecharModalDelecao() {
        metodoSendoDeletadoId = null;
        fundoModalDelecao.classList.remove('ativo');
        fundoModalDelecao.querySelector('.janela-modal').classList.remove('ativo');
        document.body.style.overflow = '';
    }

    // --- Funções de Carregamento de Dados e Tabela ---

    async function carregarMetodosPagamento() {
        gridMetodosPagamento.innerHTML = '<div class="estado-carregando">Carregando métodos...</div>';
        corpoTabelaMetodos.innerHTML = `<tr><td colspan="6" class="estado-carregando">Carregando métodos...</td></tr>`;

        try {
            const todosOsMetodos = await api.buscarMetodosPagamento({ idUsuario });
            const termoBusca = inputBusca.value.toLowerCase();

            const metodosFiltrados = todosOsMetodos.filter(metodo =>
                metodo.nome.toLowerCase().includes(termoBusca) ||
                metodo.tipo.toLowerCase().includes(termoBusca) ||
                (metodo.descricao && metodo.descricao.toLowerCase().includes(termoBusca))
            );

            // Renderizar Cartões de Resumo
            gridMetodosPagamento.innerHTML = ''; // Limpa antes de preencher
            if (metodosFiltrados.length === 0) {
                gridMetodosPagamento.innerHTML = `<div class="estado-vazio full-width"><p>Nenhum método de pagamento encontrado.</p></div>`;
            } else {
                metodosFiltrados.forEach(metodo => {
                    // Para exibir a conta vinculada no cartão, a API precisaria retornar o idConta ou numeroConta
                    // no MetodoPagamentoResponseViewModel. Por enquanto, será "N/A".
                    const contaVinculadaNome = contasDisponiveis.find(c => c.idConta === metodo.idConta)?.numeroConta || 'N/A'; // Supondo que 'metodo.idConta' exista
                    const cardHtml = `
                        <div class="cartao cartao-metodo">
                            <div class="cartao-icone">
                                <i class="fas fa-credit-card"></i>
                            </div>
                            <div class="cartao-conteudo">
                                <h3>${metodo.nome}</h3>
                                <p>${metodo.tipo} ${metodo.limite ? `(Limite: ${CONFIG.UTIL.formatarMoeda(metodo.limite)})` : ''}</p>
                                <p class="texto-secundario">Conta: ${contaVinculadaNome}</p>
                            </div>
                            <div class="acoes">
                                <button class="botao-icone" data-action="editar" data-id="${metodo.idMetodoPagamento}"><i class="fas fa-edit"></i></button>
                                <button class="botao-icone" data-action="excluir" data-id="${metodo.idMetodoPagamento}"><i class="fas fa-trash-alt"></i></button>
                            </div>
                        </div>
                    `;
                    gridMetodosPagamento.innerHTML += cardHtml;
                });
            }

            // Renderizar Tabela de Métodos de Pagamento
            corpoTabelaMetodos.innerHTML = ''; // Limpa antes de preencher
            if (metodosFiltrados.length === 0) {
                corpoTabelaMetodos.innerHTML = `<tr><td colspan="6" class="estado-vazio">Nenhum método de pagamento encontrado.</td></tr>`;
                return;
            }

            metodosFiltrados.forEach(metodo => {
                // Para exibir a conta vinculada na tabela, a API precisaria retornar o idConta ou numeroConta
                // no MetodoPagamentoResponseViewModel. Por enquanto, será "N/A".
                const contaVinculadaNome = contasDisponiveis.find(c => c.idConta === metodo.idConta)?.numeroConta || 'N/A'; // Supondo que 'metodo.idConta' exista
                const row = corpoTabelaMetodos.insertRow();
                row.innerHTML = `
                    <td>${metodo.nome}</td>
                    <td>${metodo.tipo}</td>
                    <td>${contaVinculadaNome}</td>
                    <td>${metodo.limite ? CONFIG.UTIL.formatarMoeda(metodo.limite) : 'N/A'}</td>
                    <td>${metodo.descricao || 'N/A'}</td>
                    <td class="acoes">
                        <button class="botao-icone" data-action="editar" data-id="${metodo.idMetodoPagamento}"><i class="fas fa-edit"></i></button>
                        <button class="botao-icone" data-action="excluir" data-id="${metodo.idMetodoPagamento}"><i class="fas fa-trash-alt"></i></button>
                    </td>
                `;
            });

        } catch (error) {
            console.error('Erro ao carregar métodos de pagamento:', error.message);
            gridMetodosPagamento.innerHTML = `<div class="estado-erro full-width"><p>Erro ao carregar métodos de pagamento.</p></div>`;
            corpoTabelaMetodos.innerHTML = `<tr><td colspan="6" class="estado-erro">Erro ao carregar métodos de pagamento.</td></tr>`;
            exibirNotificacao('Erro ao carregar métodos de pagamento.', 'erro');
        }
    }

    // --- Manipuladores de Eventos ---

    btnNovoMetodo.addEventListener('click', () => abrirModalMetodo());
    fecharModalMetodoBtn.addEventListener('click', fecharModalMetodo);
    btnCancelarMetodo.addEventListener('click', fecharModalMetodo);

    // Fechar modal de método de pagamento ao clicar fora
    fundoModalMetodo.addEventListener('click', (event) => {
        if (event.target === fundoModalMetodo) {
            fecharModalMetodo();
        }
    });

    // Submissão do formulário de método de pagamento
    formularioMetodo.addEventListener('submit', async (event) => {
        event.preventDefault();
        
        if (!validarFormulario(formularioMetodo)) {
            exibirNotificacao('Por favor, preencha todos os campos obrigatórios corretamente.', 'erro');
            return;
        }

        const dadosMetodo = {
            nome: inputNomeMetodo.value,
            tipo: selectTipoMetodo.value,
            limite: inputLimiteMetodo.value ? parseFloat(inputLimiteMetodo.value) : null,
            descricao: inputDescricaoMetodo.value
        };

        // A API de criar/atualizar método de pagamento espera idConta como parâmetro de query
        const params = { 
            idUsuario: idUsuario,
            idConta: parseInt(selectContaVinculada.value) // ID da conta selecionada
        };

        try {
            const metodoId = inputMetodoId.value;
            if (metodoId) {
                // Atualizar Método de Pagamento
                params.idMetodoPagamento = parseInt(metodoId);
                await api.atualizarMetodoPagamento(dadosMetodo, params);
                exibirNotificacao('Método de pagamento atualizado com sucesso!', 'sucesso');
            } else {
                // Criar Novo Método de Pagamento
                await api.criarMetodoPagamento(dadosMetodo, params);
                exibirNotificacao('Método de pagamento criado com sucesso!', 'sucesso');
            }
            fecharModalMetodo();
            carregarMetodosPagamento(); // Recarrega a lista após a operação
        } catch (error) {
            console.error('Erro ao salvar método de pagamento:', error.message);
            exibirNotificacao('Erro ao salvar método de pagamento. Verifique os dados.', 'erro');
        }
    });

    // Ações da tabela e dos cartões (editar/excluir)
    document.addEventListener('click', async (event) => {
        const target = event.target.closest('button');
        if (!target) return;

        const action = target.dataset.action;
        const idMetodoPagamento = parseInt(target.dataset.id); 

        if (action === 'editar') {
            try {
                // Para editar, precisamos dos dados completos do método.
                const todosOsMetodos = await api.buscarMetodosPagamento({ idUsuario });
                const metodoParaEditar = todosOsMetodos.find(m => m.idMetodoPagamento === idMetodoPagamento);

                if (metodoParaEditar) {
                    abrirModalMetodo(metodoParaEditar);
                } else {
                    exibirNotificacao('Método de pagamento não encontrado para edição.', 'erro');
                }
            } catch (error) {
                console.error('Erro ao buscar método de pagamento para edição:', error.message);
                exibirNotificacao('Erro ao carregar dados do método para edição.', 'erro');
            }
        } else if (action === 'excluir') {
            abrirModalDelecao(idMetodoPagamento);
        }
    });

    // Eventos do modal de deleção
    fecharModalDelecaoBtn.addEventListener('click', fecharModalDelecao);
    btnCancelarDelecao.addEventListener('click', fecharModalDelecao);
    btnConfirmarDelecao.addEventListener('click', async () => {
        if (metodoSendoDeletadoId) {
            try {
                await api.deletarMetodoPagamento({ idUsuario, idMetodoPagamento: metodoSendoDeletadoId });
                exibirNotificacao('Método de pagamento excluído com sucesso!', 'sucesso');
                fecharModalDelecao();
                carregarMetodosPagamento(); // Recarrega a lista após a exclusão
            } catch (error) {
                console.error('Erro ao excluir método de pagamento:', error.message);
                exibirNotificacao('Erro ao excluir método de pagamento.', 'erro');
            }
        }
    });

    // Fechar modal de deleção ao clicar fora
    fundoModalDelecao.addEventListener('click', (event) => {
        if (event.target === fundoModalDelecao) {
            fecharModalDelecao();
        }
    });

    // Busca com debounce para evitar muitas requisições
    inputBusca.addEventListener('keyup', CONFIG.UTIL.debounce(carregarMetodosPagamento, 300));

    // --- Inicialização ---
    // Carrega as contas primeiro para que o select de contas vinculadas possa ser populado
    // antes de carregar os métodos de pagamento.
    api.buscarContas({ idUsuario })
        .then(contas => {
            contasDisponiveis = contas;
            carregarMetodosPagamento(); // Carrega os métodos de pagamento iniciais
        })
        .catch(error => {
            console.error('Erro na inicialização: Não foi possível carregar as contas.', error.message);
            exibirNotificacao('Erro na inicialização: Não foi possível carregar as contas.', 'erro');
            carregarMetodosPagamento(); // Tenta carregar os métodos mesmo sem as contas
        });
});



