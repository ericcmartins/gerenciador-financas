import { api } from './api.js';
import { CONFIG } from './config.js';

document.addEventListener('DOMContentLoaded', () => {
    const idUsuario = localStorage.getItem('finon_user_id');
    if (!idUsuario) return;

    // --- Seleção de Elementos ---
    const btnNovaConta = document.getElementById('btnNovaConta');
    const gridContas = document.getElementById('gridContas');
    const corpoTabelaContas = document.getElementById('corpoTabelaContas');
    const inputBusca = document.getElementById('inputBusca');

    // Elementos do Modal de Conta (Adicionar/Editar)
    const fundoModalConta = document.getElementById('fundoModalConta');
    const modalConta = document.getElementById('modalConta');
    const tituloModalConta = modalConta.querySelector('#tituloModal'); // Reutiliza o ID 'tituloModal'
    const fecharModalContaBtn = document.getElementById('fecharModalConta');
    const btnCancelarConta = document.getElementById('btnCancelarConta');
    const formularioConta = document.getElementById('formularioConta');
    const inputContaId = document.getElementById('contaId'); // Campo hidden para o ID da conta
    const inputNumeroConta = document.getElementById('numeroConta');
    const selectTipoConta = document.getElementById('tipoConta');
    const inputInstituicaoConta = document.getElementById('instituicaoConta');

    // Elementos do Modal de Deleção
    const fundoModalDelecao = document.getElementById('fundoModalDelecao');
    const fecharModalDelecaoBtn = document.getElementById('fecharModalDelecao');
    const btnCancelarDelecao = document.getElementById('btnCancelarDelecao');
    const btnConfirmarDelecao = document.getElementById('btnConfirmarDelecao');

    let contaSendoDeletadaId = null; // Variável para armazenar o ID da conta a ser deletada

    // --- Funções Auxiliares (reutilizadas de outros módulos) ---

    /**
     * Valida o formulário de conta.
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
     * Abre o modal de conta (para adicionar ou editar).
     * @param {object | null} conta - Objeto conta se for edição, null se for nova.
     */
    function abrirModalConta(conta = null) {
        fundoModalConta.classList.add('ativo');
        modalConta.classList.add('ativo');
        document.body.style.overflow = 'hidden'; // Evita rolagem do corpo

        formularioConta.reset();
        limparErrosFormulario(formularioConta);
        inputContaId.value = ''; // Limpa o ID para nova conta

        if (conta) {
            tituloModalConta.textContent = 'Editar Conta';
            inputContaId.value = conta.idConta; // Supondo que a API retorne idConta
            inputNumeroConta.value = conta.numeroConta;
            selectTipoConta.value = conta.tipo;
            inputInstituicaoConta.value = conta.instituicao;
        } else {
            tituloModalConta.textContent = 'Nova Conta';
        }
    }

    /**
     * Fecha o modal de conta.
     */
    function fecharModalConta() {
        fundoModalConta.classList.remove('ativo');
        modalConta.classList.remove('ativo');
        document.body.style.overflow = ''; // Restaura a rolagem
        formularioConta.reset();
        limparErrosFormulario(formularioConta);
    }

    /**
     * Abre o modal de confirmação de deleção.
     * @param {number} id - O ID da conta a ser deletada.
     */
    function abrirModalDelecao(id) {
        contaSendoDeletadaId = id;
        fundoModalDelecao.classList.add('ativo');
        fundoModalDelecao.querySelector('.janela-modal').classList.add('ativo');
        document.body.style.overflow = 'hidden';
    }

    /**
     * Fecha o modal de confirmação de deleção.
     */
    function fecharModalDelecao() {
        contaSendoDeletadaId = null;
        fundoModalDelecao.classList.remove('ativo');
        fundoModalDelecao.querySelector('.janela-modal').classList.remove('ativo');
        document.body.style.overflow = '';
    }

    // --- Funções de Carregamento de Dados e Tabela ---

    async function carregarContas() {
        gridContas.innerHTML = '<div class="estado-carregando">Carregando contas...</div>';
        corpoTabelaContas.innerHTML = `<tr><td colspan="5" class="estado-carregando">Carregando contas...</td></tr>`;

        try {
            const todasAsContas = await api.buscarContas({ idUsuario });
            const saldosPorConta = await api.buscarSaldoPorConta({ idUsuario });

            const termoBusca = inputBusca.value.toLowerCase();

            const contasComSaldo = todasAsContas.map(conta => {
                const saldoInfo = saldosPorConta.find(s => s.numeroConta === conta.numeroConta);
                return {
                    ...conta,
                    saldoConta: saldoInfo ? saldoInfo.saldoConta : 0 // Adiciona o saldo à conta
                };
            }).filter(conta =>
                conta.numeroConta.toLowerCase().includes(termoBusca) ||
                conta.tipo.toLowerCase().includes(termoBusca) ||
                conta.instituicao.toLowerCase().includes(termoBusca)
            );

            // Renderizar Cartões de Resumo
            gridContas.innerHTML = ''; // Limpa antes de preencher
            if (contasComSaldo.length === 0) {
                gridContas.innerHTML = `<div class="estado-vazio full-width"><p>Nenhuma conta encontrada.</p></div>`;
            } else {
                contasComSaldo.forEach(conta => {
                    const saldoClasse = conta.saldoConta >= 0 ? 'texto-sucesso' : 'texto-erro';
                    const cardHtml = `
                        <div class="cartao cartao-conta">
                            <div class="cartao-icone">
                                <i class="fas fa-university"></i>
                            </div>
                            <div class="cartao-conteudo">
                                <h3>${conta.numeroConta}</h3>
                                <p>${conta.instituicao} (${conta.tipo})</p>
                                <p class="valor ${saldoClasse}">${CONFIG.UTIL.formatarMoeda(conta.saldoConta)}</p>
                            </div>
                            <div class="acoes">
                                <button class="botao-icone" data-action="editar" data-id="${conta.idConta}"><i class="fas fa-edit"></i></button>
                                <button class="botao-icone" data-action="excluir" data-id="${conta.idConta}"><i class="fas fa-trash-alt"></i></button>
                            </div>
                        </div>
                    `;
                    gridContas.innerHTML += cardHtml;
                });
            }


            // Renderizar Tabela de Contas
            corpoTabelaContas.innerHTML = ''; // Limpa antes de preencher
            if (contasComSaldo.length === 0) {
                corpoTabelaContas.innerHTML = `<tr><td colspan="5" class="estado-vazio">Nenhuma conta encontrada.</td></tr>`;
                return;
            }

            contasComSaldo.forEach(conta => {
                const saldoClasse = conta.saldoConta >= 0 ? 'texto-sucesso' : 'texto-erro';
                const row = corpoTabelaContas.insertRow();
                row.innerHTML = `
                    <td>${conta.numeroConta}</td>
                    <td>${conta.tipo}</td>
                    <td>${conta.instituicao}</td>
                    <td class="${saldoClasse}">${CONFIG.UTIL.formatarMoeda(conta.saldoConta)}</td>
                    <td class="acoes">
                        <button class="botao-icone" data-action="editar" data-id="${conta.idConta}"><i class="fas fa-edit"></i></button>
                        <button class="botao-icone" data-action="excluir" data-id="${conta.idConta}"><i class="fas fa-trash-alt"></i></button>
                    </td>
                `;
            });

        } catch (error) {
            console.error('Erro ao carregar contas:', error.message);
            gridContas.innerHTML = `<div class="estado-erro full-width"><p>Erro ao carregar contas.</p></div>`;
            corpoTabelaContas.innerHTML = `<tr><td colspan="5" class="estado-erro">Erro ao carregar contas.</td></tr>`;
            exibirNotificacao('Erro ao carregar contas.', 'erro');
        }
    }

    // --- Manipuladores de Eventos ---

    btnNovaConta.addEventListener('click', () => abrirModalConta());
    fecharModalContaBtn.addEventListener('click', fecharModalConta);
    btnCancelarConta.addEventListener('click', fecharModalConta);

    // Fechar modal de conta ao clicar fora
    fundoModalConta.addEventListener('click', (event) => {
        if (event.target === fundoModalConta) {
            fecharModalConta();
        }
    });

    // Submissão do formulário de conta
    formularioConta.addEventListener('submit', async (event) => {
        event.preventDefault();
        
        if (!validarFormulario(formularioConta)) {
            exibirNotificacao('Por favor, preencha todos os campos obrigatórios corretamente.', 'erro');
            return;
        }

        const dadosConta = {
            numeroConta: inputNumeroConta.value,
            tipo: selectTipoConta.value,
            instituicao: inputInstituicaoConta.value
        };

        const params = { idUsuario: idUsuario };

        try {
            const contaId = inputContaId.value;
            if (contaId) {
                // Atualizar Conta
                params.idConta = parseInt(contaId);
                await api.atualizarConta(dadosConta, params);
                exibirNotificacao('Conta atualizada com sucesso!', 'sucesso');
            } else {
                // Criar Nova Conta
                await api.criarConta(dadosConta, params);
                exibirNotificacao('Conta criada com sucesso!', 'sucesso');
            }
            fecharModalConta();
            carregarContas(); // Recarrega a lista após a operação
        } catch (error) {
            console.error('Erro ao salvar conta:', error.message);
            exibirNotificacao('Erro ao salvar conta. Verifique os dados.', 'erro');
        }
    });

    // Ações da tabela e dos cartões (editar/excluir)
    document.addEventListener('click', async (event) => {
        const target = event.target.closest('button');
        if (!target) return;

        const action = target.dataset.action;
        const idConta = parseInt(target.dataset.id); 

        if (action === 'editar') {
            try {
                // Para editar, precisamos dos dados completos da conta.
                const todasAsContas = await api.buscarContas({ idUsuario });
                const contaParaEditar = todasAsContas.find(c => c.idConta === idConta);

                if (contaParaEditar) {
                    abrirModalConta(contaParaEditar);
                } else {
                    exibirNotificacao('Conta não encontrada para edição.', 'erro');
                }
            } catch (error) {
                console.error('Erro ao buscar conta para edição:', error.message);
                exibirNotificacao('Erro ao carregar dados da conta para edição.', 'erro');
            }
        } else if (action === 'excluir') {
            abrirModalDelecao(idConta);
        }
    });

    // Eventos do modal de deleção
    fecharModalDelecaoBtn.addEventListener('click', fecharModalDelecao);
    btnCancelarDelecao.addEventListener('click', fecharModalDelecao);
    btnConfirmarDelecao.addEventListener('click', async () => {
        if (contaSendoDeletadaId) {
            try {
                await api.deletarConta({ idUsuario, idConta: contaSendoDeletadaId });
                exibirNotificacao('Conta excluída com sucesso!', 'sucesso');
                fecharModalDelecao();
                carregarContas(); // Recarrega a lista após a exclusão
            } catch (error) {
                console.error('Erro ao excluir conta:', error.message);
                exibirNotificacao('Erro ao excluir conta.', 'erro');
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
    inputBusca.addEventListener('keyup', CONFIG.UTIL.debounce(carregarContas, 300));

    // --- Inicialização ---
    carregarContas(); // Carrega as contas iniciais
});

