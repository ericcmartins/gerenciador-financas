import { api } from './api.js';
import { CONFIG } from './config.js';

document.addEventListener('DOMContentLoaded', () => {
    const idUsuario = localStorage.getItem('finon_user_id');
    if (!idUsuario) {
        window.location.href = 'login.html';
        return;
    }

    // --- ESTADO DA PÁGINA ---
    const estado = {
        contas: [],
        contaParaDeletarId: null,
    };

    // --- SELEÇÃO DE ELEMENTOS DO DOM ---
    const gridContas = document.getElementById('gridContas');
    const corpoTabelaContas = document.getElementById('corpoTabelaContas');
    const btnNovaConta = document.getElementById('btnNovaConta');
    const inputBusca = document.getElementById('inputBusca');
    
    // Modal
    const fundoModalConta = document.getElementById('fundoModalConta');
    const tituloModal = document.getElementById('tituloModal');
    const formularioConta = document.getElementById('formularioConta');
    const fecharModalContaBtn = document.getElementById('fecharModalConta');
    const btnCancelarConta = document.getElementById('btnCancelarConta');
    const inputContaId = document.getElementById('contaId');

    // Modal de Deleção
    const fundoModalDelecao = document.getElementById('fundoModalDelecao');
    const btnConfirmarDelecao = document.getElementById('btnConfirmarDelecao');
    const btnCancelarDelecao = document.getElementById('btnCancelarDelecao');
    const fecharModalDelecaoBtn = document.getElementById('fecharModalDelecao');


    // --- FUNÇÕES DE CARREGAMENTO E RENDERIZAÇÃO ---

    /**
     * Carrega as contas e seus respectivos saldos da API.
     */
    async function carregarContas() {
        gridContas.innerHTML = `<div class="carregando">Carregando...</div>`;
        corpoTabelaContas.innerHTML = `<tr><td colspan="5" class="carregando">Carregando...</td></tr>`;

        try {
            // Faz as duas chamadas em paralelo para mais velocidade
            const [contas, saldos] = await Promise.all([
                api.buscarContas({ idUsuario }),
                api.buscarSaldoPorConta({ idUsuario })
            ]);

            // Combina os dados de contas e saldos em um único array
            estado.contas = contas.map(conta => {
                const saldoInfo = saldos.find(s => s.numeroConta === conta.numeroConta);
                return { ...conta, saldoConta: saldoInfo ? saldoInfo.saldoConta : 0 };
            });

            renderizarConteudo();
        } catch (erro) {
            console.error("Erro ao carregar contas:", erro);
            gridContas.innerHTML = `<div class="estado-vazio-pagina">Erro ao carregar os dados.</div>`;
            corpoTabelaContas.innerHTML = `<tr><td colspan="5">Erro ao carregar os dados.</td></tr>`;
        }
    }

    /**
     * Renderiza o grid e a tabela com base nos dados do estado e no termo de busca.
     */
    function renderizarConteudo() {
        const termoBusca = inputBusca.value.toLowerCase();
        const contasFiltradas = estado.contas.filter(conta =>
            conta.numeroConta.toLowerCase().includes(termoBusca) ||
            conta.tipo.toLowerCase().includes(termoBusca) ||
            conta.instituicao.toLowerCase().includes(termoBusca)
        );
        renderizarGrid(contasFiltradas);
        renderizarTabela(contasFiltradas);
    }

    function renderizarGrid(contas) {
        gridContas.innerHTML = '';
        if (contas.length === 0) {
            gridContas.innerHTML = `<div class="estado-vazio-pagina">Nenhuma conta encontrada.</div>`;
            return;
        }

        contas.forEach(conta => {
            const saldoClasse = conta.saldoConta >= 0 ? 'positivo' : 'negativo';
            const cardHtml = `
                <div class="cartao-conta">
                    <div class="cabecalho-conta">
                        <div class="info-conta">
                            <h3>${conta.numeroConta}</h3>
                            <p class="tipo-conta">${conta.instituicao} (${conta.tipo})</p>
                        </div>
                        <div class="botoes-acao">
                            <button class="botao-acao editar" data-id="${conta.idConta}"><i class="fas fa-edit"></i></button>
                            <button class="botao-acao deletar" data-id="${conta.idConta}"><i class="fas fa-trash"></i></button>
                        </div>
                    </div>
                    <div class="saldo-conta">
                        <span class="label-saldo">Saldo</span>
                        <p class="valor-saldo ${saldoClasse}">${CONFIG.UTIL.formatarMoeda(conta.saldoConta)}</p>
                    </div>
                </div>
            `;
            gridContas.innerHTML += cardHtml;
        });
    }

    function renderizarTabela(contas) {
        corpoTabelaContas.innerHTML = '';
        if (contas.length === 0) {
            corpoTabelaContas.innerHTML = `<tr><td colspan="5" class="estado-vazio-pagina">Nenhuma conta encontrada.</td></tr>`;
            return;
        }

        contas.forEach(conta => {
            const saldoClasse = conta.saldoConta >= 0 ? 'receita' : 'despesa';
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${conta.numeroConta}</td>
                <td>${conta.tipo}</td>
                <td>${conta.instituicao}</td>
                <td class="celula-valor ${saldoClasse}">${CONFIG.UTIL.formatarMoeda(conta.saldoConta)}</td>
                <td>
                    <div class="botoes-acao">
                        <button class="botao-acao editar" data-id="${conta.idConta}"><i class="fas fa-edit"></i></button>
                        <button class="botao-acao deletar" data-id="${conta.idConta}"><i class="fas fa-trash"></i></button>
                    </div>
                </td>
            `;
            corpoTabelaContas.appendChild(tr);
        });
    }

    // --- LÓGICA DO MODAL ---

    function abrirModal(conta = null) {
        formularioConta.reset();
        inputContaId.value = '';
        tituloModal.textContent = 'Nova Conta';

        if (conta) { // Modo Edição
            tituloModal.textContent = 'Editar Conta';
            inputContaId.value = conta.idConta;
            document.getElementById('numeroConta').value = conta.numeroConta;
            document.getElementById('tipoConta').value = conta.tipo;
            document.getElementById('instituicaoConta').value = conta.instituicao;
        }
        
        fundoModalConta.classList.add('ativo');
    }

    function fecharModal() {
        fundoModalConta.classList.remove('ativo');
    }

    async function submeterFormulario(evento) {
        evento.preventDefault();
        const id = inputContaId.value;
        const dadosCorpo = {
            numeroConta: document.getElementById('numeroConta').value.trim(),
            tipo: document.getElementById('tipoConta').value,
            instituicao: document.getElementById('instituicaoConta').value.trim()
        };

        if (!dadosCorpo.numeroConta || !dadosCorpo.tipo || !dadosCorpo.instituicao) {
            alert('Por favor, preencha todos os campos obrigatórios.');
            return;
        }

        try {
            if (id) { // ATUALIZAR
                await api.atualizarConta(dadosCorpo, { idUsuario, idConta: id });
                alert('Conta atualizada com sucesso!');
            } else { // CRIAR
                await api.criarConta(dadosCorpo, { idUsuario });
                alert('Conta criada com sucesso!');
            }
            fecharModal();
            carregarContas();
        } catch (erro) {
            alert(`Erro ao salvar conta: ${erro.message}`);
        }
    }

    function abrirModalDelecao(id) {
        estado.contaParaDeletarId = id;
        fundoModalDelecao.classList.add('ativo');
    }

    function fecharModalDelecao() {
        estado.contaParaDeletarId = null;
        fundoModalDelecao.classList.remove('ativo');
    }

    async function confirmarDelecao() {
        if (!estado.contaParaDeletarId) return;
        try {
            await api.deletarConta({ idUsuario, idConta: estado.contaParaDeletarId });
            alert('Conta excluída com sucesso.');
            fecharModalDelecao();
            carregarContas();
        } catch(erro) {
            alert(`Erro ao excluir: ${erro.message}`);
        }
    }


    // --- CONFIGURAÇÃO DE EVENTOS ---
    
    function configurarEventListeners() {
        btnNovaConta.addEventListener('click', () => abrirModal());
        fecharModalContaBtn.addEventListener('click', fecharModal);
        btnCancelarConta.addEventListener('click', fecharModal);
        formularioConta.addEventListener('submit', submeterFormulario);
        inputBusca.addEventListener('keyup', CONFIG.UTIL.debounce(renderizarConteudo, 300));

        // Delegação de eventos para botões de ação
        document.querySelector('.conteudo').addEventListener('click', (evento) => {
            const botao = evento.target.closest('.botao-acao');
            if (!botao) return;

            const id = botao.dataset.id;
            const acao = botao.classList.contains('editar') ? 'editar' : 'deletar';

            if (acao === 'editar') {
                const conta = estado.contas.find(c => c.idConta == id);
                if (conta) abrirModal(conta);
            } else if (acao === 'deletar') {
                abrirModalDelecao(id);
            }
        });

        // Eventos do modal de deleção
        fecharModalDelecaoBtn.addEventListener('click', fecharModalDelecao);
        btnCancelarDelecao.addEventListener('click', fecharModalDelecao);
        btnConfirmarDelecao.addEventListener('click', confirmarDelecao);
    }
    
    // --- INICIALIZAÇÃO DA PÁGINA ---
    
    carregarContas();
    configurarEventListeners();
});