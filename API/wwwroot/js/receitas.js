import { api } from './api.js';
import { CONFIG } from './config.js';

document.addEventListener('DOMContentLoaded', () => {
    // Garante que temos o ID do usuário para fazer as requisições
    const idUsuario = localStorage.getItem('finon_user_id');
    if (!idUsuario) {
        window.location.href = 'login.html';
        return;
    }

    // Objeto para guardar o estado da página
    const estado = {
        todasAsReceitas: [],
        categorias: [],
        contas: [],
        paginaAtual: 1,
        itensPorPagina: 10, // Pode ser ajustado ou vir de CONFIG
        totalPaginas: 1,
    };

    // --- SELEÇÃO DE ELEMENTOS DO DOM ---
    const corpoTabela = document.getElementById('corpoTabelaReceitas');
    const totalReceitasEl = document.getElementById('totalReceitas');
    const receitasRecorrentesEl = document.getElementById('receitasRecorrentes');
    const mediaMensalEl = document.getElementById('mediaMensal');
    
    // Filtros
    const filtroDataEl = document.getElementById('filtroData');
    const filtroCategoriaEl = document.getElementById('filtroCategoria');
    const filtroContaEl = document.getElementById('filtroConta');
    const btnAplicarFiltros = document.getElementById('btnAplicarFiltros');

    // Modal
    const fundoModal = document.getElementById('fundoModal');
    const modalReceita = document.getElementById('modalReceita');
    const formularioReceita = document.getElementById('formularioReceita');
    const tituloModal = document.getElementById('tituloModal');
    const btnNovaReceita = document.getElementById('btnNovaReceita');
    const btnFecharModal = document.getElementById('fecharModal');
    const btnCancelarModal = document.getElementById('btnCancelar');
    const inputReceitaId = document.getElementById('receitaId'); // Campo hidden

    // Paginação
    const infoPaginaEl = document.getElementById('infoPagina');
    const btnPaginaAnterior = document.getElementById('paginaAnterior');
    const btnProximaPagina = document.getElementById('proximaPagina');


    // --- FUNÇÕES DE CARREGAMENTO E RENDERIZAÇÃO ---

    /**
     * Carrega os dados de categorias e contas para preencher os seletores de filtro e do modal.
     */
    async function carregarDadosDeSuporte() {
        try {
            const [categorias, contas] = await Promise.all([
                api.buscarCategorias({ idUsuario }),
                api.buscarContas({ idUsuario })
            ]);
            estado.categorias = categorias;
            estado.contas = contas;
            
            popularSelect(filtroCategoriaEl, categorias, 'idCategoria', 'nome', 'Todas as categorias');
            popularSelect(filtroContaEl, contas, 'idConta', 'numeroConta', 'Todas as contas');
            
            // Popula também os selects do modal
            const selectCategoriaModal = document.querySelector('#modalReceita #categoria');
            const selectContaModal = document.querySelector('#modalReceita #conta');
            popularSelect(selectCategoriaModal, categorias, 'idCategoria', 'nome', 'Selecione uma categoria');
            popularSelect(selectContaModal, contas, 'idConta', 'numeroConta', 'Selecione uma conta');
        } catch (erro) {
            console.error("Erro ao carregar dados de suporte (categorias/contas):", erro.message);
            alert("Erro ao carregar dados para os filtros.");
        }
    }
    
    /**
     * Busca as receitas na API com base nos filtros e atualiza a tela.
     */
    async function carregarReceitas() {
        corpoTabela.innerHTML = `<tr><td colspan="7" class="carregando">Carregando...</td></tr>`;
        try {
            const dias = converterPeriodoParaDias(filtroDataEl.value);
            const filtros = {
                idUsuario,
                periodo: dias,
                idCategoria: filtroCategoriaEl.value,
                idConta: filtroContaEl.value,
            };
            Object.keys(filtros).forEach(key => !filtros[key] && delete filtros[key]);

            const receitas = await api.buscarReceitas(filtros);
            estado.todasAsReceitas = receitas;
            
            renderizarTabela();
            atualizarResumo();
        } catch (erro) {
            console.error("Erro ao carregar receitas:", erro.message);
            corpoTabela.innerHTML = `<tr><td colspan="7">Erro ao carregar receitas.</td></tr>`;
        }
    }

    /**
     * Renderiza a tabela com os dados paginados.
     */
    function renderizarTabela() {
        corpoTabela.innerHTML = '';
        estado.totalPaginas = Math.ceil(estado.todasAsReceitas.length / estado.itensPorPagina);
        if (estado.totalPaginas === 0) estado.totalPaginas = 1;

        const dadosPaginados = estado.todasAsReceitas.slice(
            (estado.paginaAtual - 1) * estado.itensPorPagina,
            estado.paginaAtual * estado.itensPorPagina
        );

        if (dadosPaginados.length === 0) {
            corpoTabela.innerHTML = `<tr><td colspan="7" class="estado-vazio-pagina">Nenhuma receita encontrada.</td></tr>`;
        } else {
            dadosPaginados.forEach(receita => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td>${CONFIG.UTIL.formatarData(receita.dataReceita)}</td>
                    <td>${receita.descricao}</td>
                    <td>${receita.categoria || 'N/A'}</td>
                    <td>${receita.conta || 'N/A'}</td>
                    <td class="celula-valor receita">${CONFIG.UTIL.formatarMoeda(receita.valor)}</td>
                    <td>${receita.recorrente ? 'Sim' : 'Não'}</td>
                    <td>
                        <div class="botoes-acao">
                            <button class="botao-acao editar" data-id="${receita.idReceita}"><i class="fas fa-edit"></i></button>
                            <button class="botao-acao deletar" data-id="${receita.idReceita}"><i class="fas fa-trash"></i></button>
                        </div>
                    </td>
                `;
                corpoTabela.appendChild(tr);
            });
        }
        atualizarPaginacao();
    }
    
    /**
     * Atualiza os cards de resumo.
     */
    function atualizarResumo() {
        const total = estado.todasAsReceitas.reduce((acc, r) => acc + r.valor, 0);
        totalReceitasEl.textContent = CONFIG.UTIL.formatarMoeda(total);
        // Lógica para recorrentes e média pode ser adicionada aqui se a API fornecer os dados
        receitasRecorrentesEl.textContent = "N/A";
        mediaMensalEl.textContent = "N/A";
    }

    // --- LÓGICA DO MODAL ---

    function abrirModal(receita = null) {
        formularioReceita.reset();
        inputReceitaId.value = ''; // Limpa o ID
        tituloModal.textContent = 'Nova Receita';

        if (receita) { // Modo Edição
            tituloModal.textContent = 'Editar Receita';
            inputReceitaId.value = receita.idReceita;
            document.getElementById('valor').value = receita.valor;
            document.getElementById('descricao').value = receita.descricao;
            document.getElementById('dataReceita').value = CONFIG.UTIL.formatarDataParaInput(receita.dataReceita);
            
            // Pré-seleciona a categoria e conta
            const cat = estado.categorias.find(c => c.nome === receita.categoria);
            if(cat) document.getElementById('categoria').value = cat.idCategoria;
            
            const conta = estado.contas.find(c => c.numeroConta === receita.conta);
            if(conta) document.getElementById('conta').value = conta.idConta;
        }
        
        fundoModal.classList.add('ativo');
    }

    function fecharModal() {
        fundoModal.classList.remove('ativo');
    }

    async function submeterFormulario(evento) {
        evento.preventDefault();
        const id = inputReceitaId.value;

        const dadosCorpo = {
            valor: parseFloat(document.getElementById('valor').value),
            descricao: document.getElementById('descricao').value.trim(),
            dataReceita: document.getElementById('dataReceita').value,
        };

        const dadosParams = {
            idUsuario,
            idCategoria: document.getElementById('categoria').value,
            idConta: document.getElementById('conta').value,
        };
        
        // Validação simples
        if (!dadosCorpo.valor || !dadosCorpo.descricao || !dadosCorpo.dataReceita || !dadosParams.idCategoria || !dadosParams.idConta) {
            alert('Por favor, preencha todos os campos obrigatórios.');
            return;
        }

        try {
            if (id) { // ATUALIZAR
                dadosParams.idReceita = id;
                await api.atualizarReceita(dadosCorpo, dadosParams);
                alert('Receita atualizada com sucesso!');
            } else { // CRIAR
                await api.criarReceita(dadosCorpo, dadosParams);
                alert('Receita criada com sucesso!');
            }
            fecharModal();
            carregarReceitas();
        } catch (erro) {
            console.error('Erro ao salvar receita:', erro.message);
            alert(`Erro ao salvar: ${erro.message}`);
        }
    }


    // --- FUNÇÕES DE PAGINAÇÃO E AUXILIARES ---

    function atualizarPaginacao() {
        infoPaginaEl.textContent = `Página ${estado.paginaAtual} de ${estado.totalPaginas}`;
        btnPaginaAnterior.disabled = estado.paginaAtual === 1;
        btnProximaPagina.disabled = estado.paginaAtual === estado.totalPaginas;
    }

    function popularSelect(elemento, dados, valorKey, textoKey, placeholder) {
        elemento.innerHTML = `<option value="">${placeholder}</option>`;
        dados.forEach(item => {
            elemento.innerHTML += `<option value="${item[valorKey]}">${item[textoKey]}</option>`;
        });
    }

    function converterPeriodoParaDias(valor) {
        const hoje = new Date();
        switch (valor) {
            case '1': return hoje.getDate();
            case '3': return 90;
            case '6': return 180;
            case '12':
                const inicioAno = new Date(hoje.getFullYear(), 0, 1);
                return Math.ceil((hoje - inicioAno) / (1000 * 60 * 60 * 24));
            default: return 30;
        }
    }


    // --- CONFIGURAÇÃO DOS EVENT LISTENERS ---
    
    function configurarEventListeners() {
        btnNovaReceita.addEventListener('click', () => abrirModal());
        btnFecharModal.addEventListener('click', fecharModal);
        btnCancelarModal.addEventListener('click', fecharModal);
        formularioReceita.addEventListener('submit', submeterFormulario);
        btnAplicarFiltros.addEventListener('click', () => {
            estado.paginaAtual = 1;
            carregarReceitas();
        });

        // Paginação
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

        // Deleção e Edição usando delegação de eventos
        corpoTabela.addEventListener('click', (evento) => {
            const botao = evento.target.closest('.botao-acao');
            if (!botao) return;

            const id = botao.dataset.id;
            const acao = botao.classList.contains('editar') ? 'editar' : 'deletar';

            if (acao === 'editar') {
                const receita = estado.todasAsReceitas.find(r => r.idReceita == id);
                if (receita) abrirModal(receita);
            } else if (acao === 'deletar') {
                if (confirm('Tem certeza que deseja excluir esta receita?')) {
                    api.deletarReceita({ idUsuario, idReceita: id })
                        .then(() => {
                            alert('Receita excluída com sucesso!');
                            carregarReceitas();
                        })
                        .catch(err => alert(`Erro ao excluir: ${err.message}`));
                }
            }
        });
    }

    // --- INICIALIZAÇÃO ---
    async function init() {
        await carregarDadosDeSuporte();
        await carregarReceitas();
        configurarEventListeners();
    }

    init();
});