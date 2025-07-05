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
        metas: [],
        metaParaDeletarId: null,
    };

    // --- SELEÇÃO DE ELEMENTOS ---
    const gridMetas = document.getElementById('gridMetas');
    const nomeUsuarioEl = document.querySelector('.nome-usuario');
    const btnNovaMeta = document.getElementById('btnNovaMeta');

    // Modal de Meta
    const fundoModal = document.getElementById('fundoModal');
    const tituloModal = document.getElementById('tituloModal');
    const formularioMeta = document.getElementById('formularioMeta');
    const btnFecharModal = document.getElementById('fecharModal');
    const btnCancelar = document.getElementById('btnCancelar');
    const inputMetaId = document.getElementById('metaId');

    // Modal de Deleção
    const fundoModalDelecao = document.getElementById('fundoModalDelecao');
    const btnConfirmarDelecao = document.getElementById('btnConfirmarDelecao');
    const btnCancelarDelecao = document.getElementById('btnCancelarDelecao');
    const fecharModalDelecaoBtn = document.getElementById('fecharModalDelecao');

    // --- FUNÇÕES DE DADOS E RENDERIZAÇÃO ---

    function carregarInfoUsuario() {
        const nomeUsuario = localStorage.getItem('finon_user_name');
        if (nomeUsuario && nomeUsuarioEl) {
            nomeUsuarioEl.textContent = nomeUsuario;
        }
    }

    async function carregarMetas() {
        gridMetas.innerHTML = `<div class="carregando" style="grid-column: 1 / -1;">Carregando metas...</div>`;
        try {
            const metasDaApi = await api.buscarMetas({ idUsuario });
            estado.metas = Array.isArray(metasDaApi) ? metasDaApi : [];
            renderizarMetas();
        } catch (erro) {
            console.error('Erro ao carregar metas:', erro.message);
            gridMetas.innerHTML = `<div class="estado-vazio-pagina" style="grid-column: 1 / -1;">Erro ao carregar as metas.</div>`;
        }
    }

    function renderizarMetas() {
        gridMetas.innerHTML = ''; // Limpa a área
        
        // Verifica se a lista de metas está vazia
        if (estado.metas.length === 0) {
            // Se estiver vazia, mostra o convite para criar a primeira meta
            gridMetas.innerHTML = `
                <div class="estado-vazio-convite" style="grid-column: 1 / -1;">
                    <div class="icone-convite">
                        <i class="fas fa-bullseye"></i>
                    </div>
                    <h2>Você ainda não tem metas.</h2>
                    <p>Que tal começar a planejar seu futuro financeiro agora?</p>
                    <button class="botao botao-primario" id="btnCriarPrimeiraMeta">
                        <i class="fas fa-plus"></i> Criar minha primeira meta
                    </button>
                </div>
            `;
            // Adiciona o evento de clique ao novo botão para abrir o modal
            document.getElementById('btnCriarPrimeiraMeta').addEventListener('click', () => abrirModal());
            return; // Encerra a função aqui
        }

        // Se houver metas, continua e cria os cards
        estado.metas.forEach(meta => {
            const progresso = meta.valorAlvo > 0 ? (meta.valorAtual / meta.valorAlvo) * 100 : 0;
            const percentual = Math.min(progresso, 100).toFixed(0);

            const card = document.createElement('div');
            card.className = `cartao cartao-meta ${meta.concluida ? 'meta-concluida' : ''}`;
            card.innerHTML = `
                <div class="cartao-meta-cabecalho">
                    <h3>${meta.nome}</h3>
                    <div class="botoes-acao">
                        <button class="botao-acao editar" data-id="${meta.idMetaFinanceira}"><i class="fas fa-edit"></i></button>
                        <button class="botao-acao deletar" data-id="${meta.idMetaFinanceira}"><i class="fas fa-trash"></i></button>
                    </div>
                </div>
                <div class="cartao-meta-corpo">
                    <p style="color: #6b7280; margin-bottom: 1rem; flex-grow: 1;">${meta.descricao || 'Sem descrição.'}</p>
                    <div class="progresso-meta">
                        <div class="progresso-valores">
                            <span>${CONFIG.UTIL.formatarMoeda(meta.valorAtual)}</span>
                            <span style="font-weight: 600;">${CONFIG.UTIL.formatarMoeda(meta.valorAlvo)}</span>
                        </div>
                        <div class="progresso-barra-fundo">
                            <div class="progresso-barra-preenchimento" style="width: ${percentual}%;"></div>
                        </div>
                        <div class="progresso-valores" style="margin-top: 0.5rem;">
                            <span style="font-size: 0.8rem;">${percentual}%</span>
                            ${meta.concluida 
                                ? `<span class="selo-concluido">Concluída! <i class="fas fa-check-circle"></i></span>` 
                                : `<span style="font-size: 0.8rem;">Prazo: ${CONFIG.UTIL.formatarData(meta.dataLimite)}</span>`
                            }
                        </div>
                    </div>
                </div>
            `;
            gridMetas.appendChild(card);
        });
    }

    // --- LÓGICA DO MODAL ---

    function abrirModal(meta = null) {
        formularioMeta.reset();
        inputMetaId.value = '';
        tituloModal.textContent = 'Nova Meta Financeira';

        if (meta) { // Modo Edição
            tituloModal.textContent = 'Editar Meta Financeira';
            inputMetaId.value = meta.idMetaFinanceira;
            document.getElementById('nomeMeta').value = meta.nome;
            document.getElementById('descricaoMeta').value = meta.descricao || '';
            document.getElementById('valorAlvo').value = meta.valorAlvo;
            document.getElementById('valorAtual').value = meta.valorAtual;
            document.getElementById('dataInicio').value = CONFIG.UTIL.formatarDataParaInput(meta.dataInicio);
            document.getElementById('dataLimite').value = CONFIG.UTIL.formatarDataParaInput(meta.dataLimite);
        }

        fundoModal.classList.add('ativo');
    }

    function fecharModal() {
        fundoModal.classList.remove('ativo');
    }

    async function submeterFormulario(evento) {
        evento.preventDefault();
        const id = inputMetaId.value;
        const dadosCorpo = {
            nome: document.getElementById('nomeMeta').value.trim(),
            descricao: document.getElementById('descricaoMeta').value.trim(),
            valorAlvo: parseFloat(document.getElementById('valorAlvo').value),
            valorAtual: parseFloat(document.getElementById('valorAtual').value),
            dataInicio: document.getElementById('dataInicio').value,
            dataLimite: document.getElementById('dataLimite').value,
        };

        if (!dadosCorpo.nome || !dadosCorpo.valorAlvo || !dadosCorpo.dataInicio || !dadosCorpo.dataLimite) {
            alert('Por favor, preencha todos os campos obrigatórios (*).');
            return;
        }

        try {
            if (id) { // ATUALIZAR
                await api.atualizarMeta(dadosCorpo, { idUsuario, idMetaFinanceira: id });
                // idealmente, aqui viria uma notificação mais elegante
            } else { // CRIAR
                await api.criarMeta(dadosCorpo, { idUsuario });
                 // idealmente, aqui viria uma notificação mais elegante
            }
            fecharModal();
            carregarMetas(); // Recarrega as metas para mostrar a nova ou a atualizada
        } catch (erro) {
            alert(`Erro ao salvar a meta: ${erro.message}`);
        }
    }
    
    // --- LÓGICA DE DELEÇÃO ---

    function abrirModalDelecao(id) {
        estado.metaParaDeletarId = id;
        fundoModalDelecao.classList.add('ativo');
    }

    function fecharModalDelecao() {
        estado.metaParaDeletarId = null;
        fundoModalDelecao.classList.remove('ativo');
    }

    async function confirmarDelecao() {
        if (!estado.metaParaDeletarId) return;
        try {
            await api.deletarMeta({ idUsuario, idMetaFinanceira: estado.metaParaDeletarId });
            fecharModalDelecao();
            carregarMetas(); // Recarrega as metas para remover a que foi deletada
        } catch (erro) {
            alert(`Erro ao excluir a meta: ${erro.message}`);
        }
    }

    // --- CONFIGURAÇÃO DE EVENTOS ---
    
    function configurarEventListeners() {
        btnNovaMeta.addEventListener('click', () => abrirModal());
        btnFecharModal.addEventListener('click', fecharModal);
        btnCancelar.addEventListener('click', fecharModal);
        formularioMeta.addEventListener('submit', submeterFormulario);

        gridMetas.addEventListener('click', (evento) => {
            const botao = evento.target.closest('.botao-acao');
            if (!botao) return;

            const id = botao.dataset.id;
            const acao = botao.classList.contains('editar') ? 'editar' : 'deletar';

            if (acao === 'editar') {
                const meta = estado.metas.find(m => m.idMetaFinanceira == id);
                if (meta) abrirModal(meta);
            } else if (acao === 'deletar') {
                abrirModalDelecao(id);
            }
        });
        
        fecharModalDelecaoBtn.addEventListener('click', fecharModalDelecao);
        btnCancelarDelecao.addEventListener('click', fecharModalDelecao);
        btnConfirmarDelecao.addEventListener('click', confirmarDelecao);
    }
    
    // --- INICIALIZAÇÃO ---
    function init() {
        carregarInfoUsuario();
        carregarMetas();
        configurarEventListeners();
    }

    init();
});