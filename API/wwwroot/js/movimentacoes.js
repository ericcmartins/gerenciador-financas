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
        movimentacoes: [],
        paginaAtual: 1,
        itensPorPagina: 15,
        totalPaginas: 1,
    };

    // --- SELEÇÃO DE ELEMENTOS ---
    const nomeUsuarioEl = document.querySelector('.nome-usuario');
    const corpoTabela = document.getElementById('corpoTabelaMovimentacoes');
    
    // Filtros
    const filtroTipoEl = document.getElementById('filtroTipo');
    const filtroDataInicioEl = document.getElementById('filtroDataInicio');
    const filtroDataFimEl = document.getElementById('filtroDataFim');
    const btnAplicarFiltros = document.getElementById('btnAplicarFiltros');

    // Paginação
    const infoPaginaEl = document.getElementById('infoPagina');
    const btnPaginaAnterior = document.getElementById('paginaAnterior');
    const btnProximaPagina = document.getElementById('proximaPagina');

    // --- FUNÇÕES DE DADOS E RENDERIZAÇÃO ---

    function carregarInfoUsuario() {
        const nomeUsuario = localStorage.getItem('finon_user_name');
        if (nomeUsuario && nomeUsuarioEl) {
            nomeUsuarioEl.textContent = nomeUsuario;
        }
    }

    async function carregarMovimentacoes() {
        btnAplicarFiltros.disabled = true;
        btnAplicarFiltros.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Filtrando...';
        corpoTabela.innerHTML = `<tr><td colspan="6" class="carregando">Carregando movimentações...</td></tr>`;

        try {
            const params = {
                idUsuario,
                tipoMovimentacao: filtroTipoEl.value || null,
                dataInicio: filtroDataInicioEl.value || null,
                dataFim: filtroDataFimEl.value || null,
            };
            
            // Remove chaves nulas ou vazias do objeto de parâmetros
            Object.keys(params).forEach(key => (params[key] == null || params[key] === '') && delete params[key]);

            const movimentacoesDaApi = await api.buscarMovimentacoes(params);
            estado.movimentacoes = Array.isArray(movimentacoesDaApi) ? movimentacoesDaApi : [];
            estado.paginaAtual = 1; // Reseta para a primeira página a cada novo filtro
            renderizarTabela();
        } catch (erro) {
            console.error('Erro ao carregar movimentações:', erro.message);
            corpoTabela.innerHTML = `<td colspan="6" class="estado-vazio-pagina">Erro ao carregar o extrato.</td>`;
        } finally {
            btnAplicarFiltros.disabled = false;
            btnAplicarFiltros.innerHTML = '<i class="fas fa-filter"></i> Filtrar';
        }
    }

    function renderizarTabela() {
        corpoTabela.innerHTML = '';
        estado.totalPaginas = Math.ceil(estado.movimentacoes.length / estado.itensPorPagina) || 1;
        
        const dadosPaginados = estado.movimentacoes.slice(
            (estado.paginaAtual - 1) * estado.itensPorPagina,
            estado.paginaAtual * estado.itensPorPagina
        );

        if (dadosPaginados.length === 0) {
            corpoTabela.innerHTML = `<tr><td colspan="6" class="estado-vazio-pagina">Nenhuma movimentação encontrada para os filtros selecionados.</td></tr>`;
            atualizarPaginacao();
            return;
        }

        dadosPaginados.forEach(mov => {
            const eReceita = mov.tipoMovimentacao.toLowerCase() === 'receita';
            const tipoClasse = eReceita ? 'receita' : 'despesa';
            const valorFormatado = (eReceita ? '+' : '-') + CONFIG.UTIL.formatarMoeda(Math.abs(mov.valor));

            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td><span class="selo-status ${tipoClasse}">${mov.tipoMovimentacao}</span></td>
                <td>${CONFIG.UTIL.formatarData(mov.dataMovimentacao)}</td>
                <td>${mov.descricao || '-'}</td>
                <td>${mov.numeroContaOrigem || 'N/A'}</td>
                <td>${mov.categoria || 'N/A'}</td>
                <td class="celula-valor ${tipoClasse}">${valorFormatado}</td>
            `;
            corpoTabela.appendChild(tr);
        });
        
        atualizarPaginacao();
    }

    function atualizarPaginacao() {
        infoPaginaEl.textContent = `Página ${estado.paginaAtual} de ${estado.totalPaginas}`;
        btnPaginaAnterior.disabled = estado.paginaAtual === 1;
        btnProximaPagina.disabled = estado.paginaAtual >= estado.totalPaginas;
    }

    // --- CONFIGURAÇÃO DE EVENTOS ---
    
    function configurarEventListeners() {
        btnAplicarFiltros.addEventListener('click', carregarMovimentacoes);

        btnPaginaAnterior.addEventListener('click', () => {
            if (estado.paginaAtual > 1) {
                estado.paginaAtual--;
                renderizarTabela();
            }
        });

        btnProximaPagina.addEventListener('click', () => {
            if (estado.paginaAtual < estado.totalPaginas) {
                estado.paginaAtual++;
                renderizarTabela();
            }
        });
    }
    
    // --- INICIALIZAÇÃO ---
    function init() {
        carregarInfoUsuario();
        carregarMovimentacoes();
        configurarEventListeners();
    }

    init();
});
