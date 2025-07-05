import { api } from './api.js';
import { CONFIG } from './config.js';

// --- FUNÇÕES AUXILIARES DE FORMATAÇÃO ---
function formatarDataParaExibicao(dataISO) {
    if (!dataISO) return 'N/A';
    const data = new Date(dataISO);
    return data.toLocaleDateString('pt-BR', { timeZone: 'UTC' });
}

function formatarDataParaInput(dataISO) {
    if (!dataISO) return '';
    const data = new Date(dataISO);
    return data.toISOString().split('T')[0];
}

document.addEventListener('DOMContentLoaded', () => {
    // Garante que temos o ID do usuário
    const idUsuario = localStorage.getItem('finon_user_id');
    if (!idUsuario) {
        window.location.href = 'login.html';
        return;
    }

    // --- ESTADO DA PÁGINA ---
    const estado = {
        listaDeReceitas: [],
        categorias: [],
        contas: [],
        paginaAtual: 1,
        itensPorPagina: 10,
        totalPaginas: 1,
        graficoInstance: null, // Armazena a instância do gráfico para poder destruí-la
    };

    // --- SELEÇÃO DE ELEMENTOS DO DOM ---
    const corpoTabela = document.getElementById('corpoTabelaReceitas');
    const totalReceitasEl = document.getElementById('totalReceitas');
    const filtroDataEl = document.getElementById('filtroData');
    const btnAplicarFiltros = document.getElementById('btnAplicarFiltros');
    const canvasGrafico = document.getElementById('graficoReceitasCategoria').getContext('2d');
    const resumoPorContaEl = document.getElementById('resumoPorConta');
    const resumoPorCategoriaEl = document.getElementById('resumoPorCategoria');

    // Modal
    const fundoModal = document.getElementById('fundoModal');
    const formularioReceita = document.getElementById('formularioReceita');
    const tituloModal = document.getElementById('tituloModal');
    const btnNovaReceita = document.getElementById('btnNovaReceita');
    const btnFecharModal = document.getElementById('fecharModal');
    const btnCancelarModal = document.getElementById('btnCancelar');
    const inputReceitaId = document.getElementById('receitaId');

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
                <span class="celula-valor receita">${CONFIG.UTIL.formatarMoeda(item[chaveValor] || 0)}</span>
            `;
            lista.appendChild(li);
        });
        elemento.appendChild(lista);
    }
    
    function renderizarGrafico(dadosCategoria) {
        if (estado.graficoInstance) {
            estado.graficoInstance.destroy(); // Destrói o gráfico antigo
        }

        if (!dadosCategoria || dadosCategoria.length === 0) {
            const canvas = document.getElementById('graficoReceitasCategoria');
            const ctx = canvas.getContext('2d');
            ctx.clearRect(0, 0, canvas.width, canvas.height);
            ctx.font = "16px 'Segoe UI'";
            ctx.fillStyle = '#6b7280';
            ctx.textAlign = 'center';
            ctx.fillText('Sem dados para exibir no gráfico.', canvas.width / 2, canvas.height / 2);
            return;
        }

        const labels = dadosCategoria.map(d => d.categoria);
        const data = dadosCategoria.map(d => d.totalReceita || 0);

        estado.graficoInstance = new Chart(canvasGrafico, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Receitas',
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

    function renderizarTabelaReceitas() {
        corpoTabela.innerHTML = '';
        estado.totalPaginas = Math.ceil(estado.listaDeReceitas.length / estado.itensPorPagina) || 1;
        estado.paginaAtual = Math.min(estado.paginaAtual, estado.totalPaginas);

        const dadosPaginados = estado.listaDeReceitas.slice(
            (estado.paginaAtual - 1) * estado.itensPorPagina,
            estado.paginaAtual * estado.itensPorPagina
        );

        if (dadosPaginados.length === 0) {
            corpoTabela.innerHTML = `<tr><td colspan="6" class="estado-vazio-pagina" style="padding: 2rem; text-align: center;">Nenhum lançamento de receita para o período.</td></tr>`;
        } else {
            dadosPaginados.forEach(receita => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td>${formatarDataParaExibicao(receita.dataReceita)}</td>
                    <td>${receita.descricao}</td>
                    <td>${receita.categoria || 'N/A'}</td>
                    <td>${receita.conta || 'N/A'}</td>
                    <td class="celula-valor receita">${CONFIG.UTIL.formatarMoeda(receita.valor)}</td>
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

    // --- FUNÇÃO PRINCIPAL DE CARREGAMENTO (MAIS ROBUSTA) ---
    async function carregarDadosDaPagina() {
        const periodo = filtroDataEl.value;
        btnAplicarFiltros.disabled = true;
        btnAplicarFiltros.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Analisando...';

        try {
            // Usando Promise.all para carregar em paralelo onde for possível
            const [respostaTotal, respostaCategoria, respostaConta, respostaLista] = await Promise.all([
                api.buscarTotalReceitas({ idUsuario, periodo }),
                api.buscarReceitasPorCategoria({ idUsuario, periodo }),
                api.buscarReceitasPorConta({ idUsuario, periodo }),
                api.buscarReceitas({ idUsuario, periodo })
            ]);

            // 1. Processar Total
            const totalValor = parseFloat(respostaTotal) || 0;
            totalReceitasEl.textContent = CONFIG.UTIL.formatarMoeda(totalValor);

            // 2. Processar por Categoria
            const categoriaData = Array.isArray(respostaCategoria) ? respostaCategoria : [];
            renderizarGrafico(categoriaData);
            renderizarResumo(resumoPorCategoriaEl, categoriaData, 'categoria', 'totalReceita', 'Sem dados de categorias.');

            // 3. Processar por Conta
            const contaData = Array.isArray(respostaConta) ? respostaConta : [];
            renderizarResumo(resumoPorContaEl, contaData, 'numeroConta', 'totalReceita', 'Sem dados de contas.');
            
            // 4. Processar Lista de Receitas
            estado.listaDeReceitas = Array.isArray(respostaLista) ? respostaLista : [];
            renderizarTabelaReceitas();

        } catch (error) {
            console.error("ERRO DURANTE O CARREGAMENTO:", error);
            alert("Não foi possível carregar todos os dados. Verifique o console do navegador (F12) para mais detalhes.");
        } finally {
            btnAplicarFiltros.disabled = false;
            btnAplicarFiltros.innerHTML = '<i class="fas fa-filter"></i> Analisar Período';
        }
    }
    
    // --- LÓGICA DO MODAL ---
    async function carregarDadosDeSuporte() {
        try {
            const [categorias, contas] = await Promise.all([
                api.buscarCategorias({ idUsuario }),
                api.buscarContas({ idUsuario })
            ]);

            // === CÓDIGO REVERTIDO PARA A VERSÃO ORIGINAL E CORRETA ===
            // Agora que a API retorna um array, esta linha simples é suficiente e correta.
            estado.categorias = Array.isArray(categorias) ? categorias : [];
            // === FIM DA CORREÇÃO ===
            
            estado.contas = Array.isArray(contas) ? contas : [];
            
            const selectCategoriaModal = document.querySelector('#modalReceita #categoria');
            const selectContaModal = document.querySelector('#modalReceita #conta');
            popularSelect(selectCategoriaModal, estado.categorias, 'idCategoria', 'nome', 'Selecione uma categoria');
            popularSelect(selectContaModal, estado.contas, 'idConta', 'numeroConta', 'Selecione uma conta');
        } catch (erro) {
            console.error("Erro ao carregar dados de suporte (categorias/contas):", erro.message);
            alert("Erro ao carregar dados para os formulários. A criação de novas receitas pode não funcionar.");
        }
    }

    function abrirModal(receita = null) {
        formularioReceita.reset();
        inputReceitaId.value = '';
        tituloModal.textContent = 'Nova Receita';

        if (receita) {
            tituloModal.textContent = 'Editar Receita';
            inputReceitaId.value = receita.idReceita;
            document.getElementById('valor').value = receita.valor;
            document.getElementById('descricao').value = receita.descricao;
            document.getElementById('dataReceita').value = formatarDataParaInput(receita.dataReceita);
            
            const categoriaEncontrada = estado.categorias.find(c => c.nome === receita.categoria);
            const contaEncontrada = estado.contas.find(c => c.numeroConta === receita.conta);

            document.getElementById('categoria').value = categoriaEncontrada ? categoriaEncontrada.idCategoria : '';
            document.getElementById('conta').value = contaEncontrada ? contaEncontrada.idConta : '';
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
        
        if (!dadosCorpo.valor || !dadosCorpo.descricao || !dadosCorpo.dataReceita || !dadosParams.idCategoria || !dadosParams.idConta) {
            alert('Por favor, preencha todos os campos obrigatórios.');
            return;
        }

        try {
            if (id) {
                dadosParams.idReceita = id;
                await api.atualizarReceita(dadosCorpo, dadosParams);
                alert('Receita atualizada com sucesso!');
            } else {
                await api.criarReceita(dadosCorpo, dadosParams);
                alert('Receita criada com sucesso!');
            }
            fecharModal();
            carregarDadosDaPagina();
        } catch (erro) {
            console.error('Erro ao salvar receita:', erro.message);
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
        btnNovaReceita.addEventListener('click', () => abrirModal());
        btnFecharModal.addEventListener('click', fecharModal);
        btnCancelarModal.addEventListener('click', fecharModal);
        formularioReceita.addEventListener('submit', submeterFormulario);

        btnPaginaAnterior.addEventListener('click', () => {
            if (estado.paginaAtual > 1) {
                estado.paginaAtual--;
                renderizarTabelaReceitas();
            }
        });
        btnProximaPagina.addEventListener('click', () => {
            if (estado.paginaAtual < estado.totalPaginas) {
                estado.paginaAtual++;
                renderizarTabelaReceitas();
            }
        });

        corpoTabela.addEventListener('click', async (evento) => {
            const botao = evento.target.closest('.botao-acao');
            if (!botao) return;

            const id = botao.dataset.id;
            const acao = botao.classList.contains('editar') ? 'editar' : 'deletar';

            if (acao === 'editar') {
                const receita = estado.listaDeReceitas.find(r => r.idReceita == id);
                if (receita) abrirModal(receita);
            } else if (acao === 'deletar') {
                if (confirm('Tem certeza que deseja excluir esta receita?')) {
                    try {
                        await api.deletarReceita({ idUsuario, idReceita: id });
                        alert('Receita excluída com sucesso!');
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