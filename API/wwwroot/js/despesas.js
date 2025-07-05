import { api } from './api.js';
import { CONFIG } from './config.js';

document.addEventListener('DOMContentLoaded', () => {
    // Garante que temos o ID do usuário
    const idUsuario = localStorage.getItem('finon_user_id');
    if (!idUsuario) {
        window.location.href = 'login.html';
        return;
    }

    // --- ESTADO DA PÁGINA ---
    const estado = {
        listaDeDespesas: [],
        categorias: [],
        contas: [],
        metodosPagamento: [],
        paginaAtual: 1,
        itensPorPagina: 10,
        totalPaginas: 1,
        graficoInstance: null,
    };

    // --- SELEÇÃO DE ELEMENTOS DO DOM ---
    const corpoTabela = document.getElementById('corpoTabelaDespesas');
    const totalDespesasEl = document.getElementById('totalDespesas');
    const filtroDataEl = document.getElementById('filtroData');
    const btnAplicarFiltros = document.getElementById('btnAplicarFiltros');
    const canvasGrafico = document.getElementById('graficoDespesasCategoria').getContext('2d');
    const resumoPorContaEl = document.getElementById('resumoPorConta');
    const resumoPorMetodoEl = document.getElementById('resumoPorMetodo');

    // Modal
    const fundoModal = document.getElementById('fundoModal');
    const formularioDespesa = document.getElementById('formularioDespesa');
    const tituloModal = document.getElementById('tituloModal');
    const btnNovaDespesa = document.getElementById('btnNovaDespesa');
    const btnFecharModal = document.getElementById('fecharModal');
    const btnCancelarModal = document.getElementById('btnCancelar');
    const inputDespesaId = document.getElementById('despesaId');

    // Paginação
    const infoPaginaEl = document.getElementById('infoPagina');
    const btnPaginaAnterior = document.getElementById('paginaAnterior');
    const btnProximaPagina = document.getElementById('proximaPagina');

    // --- FUNÇÕES DE RENDERIZAÇÃO DA INTERFACE ---

    function renderizarResumo(elemento, dados, chaveNome, chaveValor, tituloVazio) {
        elemento.innerHTML = '';
        if (!dados || dados.length === 0) {
            elemento.innerHTML = `<p style="color: #6b7280; text-align: center;">${tituloVazio}</p>`;
            return;
        }
        const lista = document.createElement('ul');
        lista.style.listStyle = 'none';
        lista.style.padding = '0';
        
        dados.forEach(item => {
            const li = document.createElement('li');
            li.style.display = 'flex';
            li.style.justifyContent = 'space-between';
            li.style.padding = '0.5rem 0';
            li.style.borderBottom = '1px solid #f3f4f6';
            li.innerHTML = `
                <span>${item[chaveNome]}</span>
                <span class="celula-valor despesa">${CONFIG.UTIL.formatarMoeda(item[chaveValor] || 0)}</span>
            `;
            lista.appendChild(li);
        });
        elemento.appendChild(lista);
    }
    
    function renderizarGrafico(dadosCategoria) {
        if (estado.graficoInstance) {
            estado.graficoInstance.destroy();
        }

        if (!dadosCategoria || dadosCategoria.length === 0) {
            const canvas = document.getElementById('graficoDespesasCategoria');
            const ctx = canvas.getContext('2d');
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            ctx.font = "16px 'Segoe UI'";
            ctx.fillStyle = '#6b7280';
            ctx.textAlign = 'center';
            ctx.fillText('Sem dados para exibir no gráfico.', canvas.width / 2, canvas.height / 2);
            return;
        }

        const labels = dadosCategoria.map(d => d.categoria);
        const data = dadosCategoria.map(d => d.totalDespesa || 0);

        estado.graficoInstance = new Chart(canvasGrafico, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Despesas',
                    data: data,
                    backgroundColor: CONFIG.UTIL.gerarCores(data.length),
                    borderColor: '#fff',
                    borderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: 'top' },
                    tooltip: {
                        callbacks: {
                            label: (context) => `${context.label}: ${CONFIG.UTIL.formatarMoeda(context.parsed)}`
                        }
                    }
                }
            }
        });
    }

    function renderizarTabelaDespesas() {
        corpoTabela.innerHTML = '';
        estado.totalPaginas = Math.ceil(estado.listaDeDespesas.length / estado.itensPorPagina) || 1;
        estado.paginaAtual = Math.min(estado.paginaAtual, estado.totalPaginas);

        const dadosPaginados = estado.listaDeDespesas.slice(
            (estado.paginaAtual - 1) * estado.itensPorPagina,
            estado.paginaAtual * estado.itensPorPagina
        );

        if (dadosPaginados.length === 0) {
            corpoTabela.innerHTML = `<tr><td colspan="7" class="estado-vazio-pagina" style="padding: 2rem; text-align: center;">Nenhum lançamento de despesa para o período.</td></tr>`;
        } else {
            dadosPaginados.forEach(despesa => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td>${CONFIG.UTIL.formatarData(despesa.dataDespesa)}</td>
                    <td>${despesa.descricao}</td>
                    <td>${despesa.categoria || 'N/A'}</td>
                    <td>${despesa.conta || 'N/A'}</td>
                    <td>${despesa.metodoPagamento || 'N/A'}</td>
                    <td class="celula-valor despesa">${CONFIG.UTIL.formatarMoeda(despesa.valor)}</td>
                    <td>
                        <div class="botoes-acao">
                            <button class="botao-acao editar" data-id="${despesa.idDespesa}"><i class="fas fa-edit"></i></button>
                            <button class="botao-acao deletar" data-id="${despesa.idDespesa}"><i class="fas fa-trash"></i></button>
                        </div>
                    </td>
                `;
                corpoTabela.appendChild(tr);
            });
        }
        atualizarPaginacao();
    }

    // --- FUNÇÃO PRINCIPAL DE CARREGAMENTO ---
    async function carregarDadosDaPagina() {
        const periodo = filtroDataEl.value;
        btnAplicarFiltros.disabled = true;
        btnAplicarFiltros.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Analisando...';

        try {
            const params = { idUsuario, periodo };
            
            const [
                respostaTotal, 
                respostaCategoria, 
                respostaConta, 
                respostaMetodo,
                respostaLista
            ] = await Promise.all([
                api.buscarTotalDespesas(params),
                api.buscarDespesasPorCategoria(params),
                api.buscarDespesasPorConta(params),
                api.buscarDespesasPorMetodoPagamento(params),
                api.buscarDespesas(params)
            ]);

            const totalValor = parseFloat(respostaTotal) || 0;
            totalDespesasEl.textContent = CONFIG.UTIL.formatarMoeda(totalValor);

            const categoriaData = Array.isArray(respostaCategoria) ? respostaCategoria : [];
            renderizarGrafico(categoriaData);

            const contaData = Array.isArray(respostaConta) ? respostaConta : [];
            renderizarResumo(resumoPorContaEl, contaData, 'numeroConta', 'totalDespesa', 'Sem dados de contas.');
            
            const metodoData = Array.isArray(respostaMetodo) ? respostaMetodo : [];
            renderizarResumo(resumoPorMetodoEl, metodoData, 'metodoPagamento', 'totalDespesa', 'Sem dados de métodos.');

            estado.listaDeDespesas = Array.isArray(respostaLista) ? respostaLista : [];
            renderizarTabelaDespesas();

        } catch (error) {
            console.error("ERRO DURANTE O CARREGAMENTO DAS DESPESAS:", error);
            alert("Não foi possível carregar os dados das despesas. Verifique o console para mais detalhes.");
        } finally {
            btnAplicarFiltros.disabled = false;
            btnAplicarFiltros.innerHTML = '<i class="fas fa-filter"></i> Analisar Período';
        }
    }
    
    // --- LÓGICA DO MODAL ---
    async function carregarDadosDeSuporte() {
        try {
            const [categorias, contas, metodos] = await Promise.all([
                api.buscarCategorias({ idUsuario }),
                api.buscarContas({ idUsuario }),
                api.buscarMetodosPagamento({ idUsuario })
            ]);
            
            estado.categorias = Array.isArray(categorias) ? categorias : [];
            estado.contas = Array.isArray(contas) ? contas : [];
            estado.metodosPagamento = Array.isArray(metodos) ? metodos : [];
            
            const selectCategoriaModal = document.querySelector('#modalDespesa #categoria');
            const selectContaModal = document.querySelector('#modalDespesa #conta');
            const selectMetodoModal = document.querySelector('#modalDespesa #metodoPagamento');

            popularSelect(selectCategoriaModal, estado.categorias, 'idCategoria', 'nome', 'Selecione uma categoria');
            popularSelect(selectContaModal, estado.contas, 'idConta', 'numeroConta', 'Selecione uma conta');
            popularSelect(selectMetodoModal, estado.metodosPagamento, 'idMetodo', 'nome', 'Selecione um método');

        } catch (erro) {
            console.error("Erro ao carregar dados de suporte:", erro.message);
            alert("Não foi possível carregar os dados para os formulários.");
        }
    }

    function abrirModal(despesa = null) {
        formularioDespesa.reset();
        inputDespesaId.value = '';
        tituloModal.textContent = 'Nova Despesa';

        if (despesa) {
            tituloModal.textContent = 'Editar Despesa';
            inputDespesaId.value = despesa.idDespesa;
            document.getElementById('valor').value = despesa.valor;
            document.getElementById('descricao').value = despesa.descricao;
            document.getElementById('dataDespesa').value = CONFIG.UTIL.formatarDataParaInput(despesa.dataDespesa);
            
            const cat = estado.categorias.find(c => c.nome === despesa.categoria);
            const conta = estado.contas.find(c => c.numeroConta === despesa.conta);
            const metodo = estado.metodosPagamento.find(m => m.nome === despesa.metodoPagamento);

            document.getElementById('categoria').value = cat ? cat.idCategoria : '';
            document.getElementById('conta').value = conta ? conta.idConta : '';
            document.getElementById('metodoPagamento').value = metodo ? metodo.idMetodo : '';
        }
        
        fundoModal.classList.add('ativo');
    }

    function fecharModal() {
        fundoModal.classList.remove('ativo');
    }

    async function submeterFormulario(evento) {
        evento.preventDefault();
        const id = inputDespesaId.value;
        const dadosCorpo = {
            valor: parseFloat(document.getElementById('valor').value),
            descricao: document.getElementById('descricao').value.trim(),
            dataDespesa: document.getElementById('dataDespesa').value,
        };
        const dadosParams = {
            idUsuario,
            idCategoria: document.getElementById('categoria').value,
            idConta: document.getElementById('conta').value,
            idMetodoPagamento: document.getElementById('metodoPagamento').value,
        };
        
        if (!dadosCorpo.valor || !dadosCorpo.descricao || !dadosCorpo.dataDespesa || !dadosParams.idCategoria || !dadosParams.idConta || !dadosParams.idMetodoPagamento) {
            alert('Por favor, preencha todos os campos obrigatórios.');
            return;
        }

        try {
            if (id) {
                dadosParams.idDespesa = id;
                await api.atualizarDespesa(dadosCorpo, dadosParams);
                alert('Despesa atualizada com sucesso!');
            } else {
                await api.criarDespesa(dadosCorpo, dadosParams);
                alert('Despesa criada com sucesso!');
            }
            fecharModal();
            carregarDadosDaPagina();
        } catch (erro) {
            console.error('Erro ao salvar despesa:', erro.message);
            alert(`Erro ao salvar: ${erro.message}`);
        }
    }

    // --- FUNÇÕES DE PAGINAÇÃO E OUTRAS ---
    function atualizarPaginacao() {
        infoPaginaEl.textContent = `Página ${estado.paginaAtual} de ${estado.totalPaginas}`;
        btnPaginaAnterior.disabled = estado.paginaAtual === 1;
        btnProximaPagina.disabled = estado.paginaAtual >= estado.totalPaginas;
    }

    function popularSelect(elemento, dados, valorKey, textoKey, placeholder) {
        elemento.innerHTML = `<option value="">${placeholder}</option>`;
        if (Array.isArray(dados)) {
            dados.forEach(item => {
                elemento.innerHTML += `<option value="${item[valorKey]}">${item[textoKey]}</option>`;
            });
        }
    }

    // --- CONFIGURAÇÃO DOS EVENT LISTENERS ---
    function configurarEventListeners() {
        btnAplicarFiltros.addEventListener('click', carregarDadosDaPagina);
        btnNovaDespesa.addEventListener('click', () => abrirModal());
        btnFecharModal.addEventListener('click', fecharModal);
        btnCancelarModal.addEventListener('click', fecharModal);
        formularioDespesa.addEventListener('submit', submeterFormulario);

        btnPaginaAnterior.addEventListener('click', () => {
            if (estado.paginaAtual > 1) {
                estado.paginaAtual--;
                renderizarTabelaDespesas();
            }
        });
        btnProximaPagina.addEventListener('click', () => {
            if (estado.paginaAtual < estado.totalPaginas) {
                estado.paginaAtual++;
                renderizarTabelaDespesas();
            }
        });

        corpoTabela.addEventListener('click', async (evento) => {
            const botao = evento.target.closest('.botao-acao');
            if (!botao) return;

            const id = botao.dataset.id;
            const acao = botao.classList.contains('editar') ? 'editar' : 'deletar';

            if (acao === 'editar') {
                const despesa = estado.listaDeDespesas.find(d => d.idDespesa == id);
                if (despesa) abrirModal(despesa);
            } else if (acao === 'deletar') {
                if (confirm('Tem certeza que deseja excluir esta despesa?')) {
                    try {
                        await api.deletarDespesa({ idUsuario, idDespesa: id });
                        alert('Despesa excluída com sucesso!');
                        carregarDadosDaPagina();
                    } catch(err) {
                        alert(`Erro ao excluir: ${err.message}`);
                    }
                }
            }
        });
    }

    // --- INICIALIZAÇÃO ---
    async function init() {
        await carregarDadosDeSuporte();
        await carregarDadosDaPagina();
        configurarEventListeners();
    }

    init();
});
