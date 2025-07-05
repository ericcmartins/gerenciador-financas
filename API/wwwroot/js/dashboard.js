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
        categorias: [],
        contas: [],
        metodosPagamento: [],
        graficoReceitasInstance: null,
        graficoDespesasInstance: null,
        tipoTransacaoModal: null, // 'receita' ou 'despesa'
    };

    // --- SELEÇÃO DE ELEMENTOS ---
    const nomeUsuarioEl = document.querySelector('.nome-usuario');
    const totalReceitasEl = document.getElementById('totalReceitas');
    const totalDespesasEl = document.getElementById('totalDespesas');
    const saldoPeriodoEl = document.getElementById('saldoPeriodo');
    const saldoTotalGeralEl = document.getElementById('saldoTotalGeral');
    const listaTransacoesEl = document.getElementById('listaTransacoes');
    const selecaoPeriodoEl = document.getElementById('selecaoPeriodo');
    
    // Gráficos
    const ctxReceitas = document.getElementById('graficoReceitas').getContext('2d');
    const ctxDespesas = document.getElementById('graficoDespesas').getContext('2d');

    // Modal de Transação
    const fundoModal = document.getElementById('fundoModal');
    const modalTransacao = document.getElementById('modalTransacao');
    const tituloModal = document.getElementById('tituloModal');
    const formularioTransacao = document.getElementById('formularioTransacao');
    const grupoMetodoPagamento = document.getElementById('grupoMetodoPagamento');
    const btnFecharModal = document.getElementById('fecharModal');
    const btnCancelarModal = document.getElementById('cancelarModal');
    
    // Botões de Ação Rápida
    const btnNovaReceita = document.querySelector('.botao-receita');
    const btnNovaDespesa = document.querySelector('.botao-despesa');

    // --- FUNÇÕES DE CARREGAMENTO DE DADOS (MAIS ROBUSTAS) ---

    function carregarInfoUsuario() {
        const nomeUsuario = localStorage.getItem('finon_user_name');
        if (nomeUsuario && nomeUsuarioEl) {
            nomeUsuarioEl.textContent = nomeUsuario;
        }
    }

    async function carregarDadosDashboard() {
        selecaoPeriodoEl.disabled = true;
        
        const periodo = selecaoPeriodoEl.value;
        const params = { idUsuario, periodo };

        // Carrega cada parte do dashboard independentemente
        await Promise.all([
            carregarCardsResumo(params),
            carregarGraficos(params),
            carregarTransacoesRecentes({ idUsuario, periodo: 30 }) // Lista sempre dos últimos 30 dias
        ]);
        
        selecaoPeriodoEl.disabled = false;
    }

    async function carregarCardsResumo(params) {
        try {
            const [totalReceitas, totalDespesas, saldoGeral] = await Promise.all([
                api.buscarTotalReceitas(params),
                api.buscarTotalDespesas(params),
                api.buscarSaldoTotal({ idUsuario: params.idUsuario })
            ]);

            const receitasValor = parseFloat(totalReceitas) || 0;
            const despesasValor = parseFloat(totalDespesas) || 0;
            
            totalReceitasEl.textContent = CONFIG.UTIL.formatarMoeda(receitasValor);
            totalDespesasEl.textContent = CONFIG.UTIL.formatarMoeda(despesasValor);
            saldoPeriodoEl.textContent = CONFIG.UTIL.formatarMoeda(receitasValor - despesasValor);
            
            const saldoGeralValor = saldoGeral.length > 0 ? saldoGeral[0].saldoTotal : 0;
            saldoTotalGeralEl.textContent = CONFIG.UTIL.formatarMoeda(saldoGeralValor);
        } catch (erro) {
            console.error("Erro ao carregar cards de resumo:", erro);
        }
    }

    async function carregarGraficos(params) {
        try {
            const [dadosReceitas, dadosDespesas] = await Promise.all([
                api.buscarReceitasPorCategoria(params),
                api.buscarDespesasPorCategoria(params)
            ]);
            
            estado.graficoReceitasInstance = renderizarGrafico(ctxReceitas, estado.graficoReceitasInstance, 'Receitas', dadosReceitas, 'categoria', 'totalReceita');
            estado.graficoDespesasInstance = renderizarGrafico(ctxDespesas, estado.graficoDespesasInstance, 'Despesas', dadosDespesas, 'categoria', 'totalDespesa');
        } catch (erro) {
            console.error("Erro ao carregar gráficos:", erro);
        }
    }

    async function carregarTransacoesRecentes(params) {
        try {
            const transacoes = await api.buscarMovimentacoes(params);
            renderizarTransacoes(transacoes);
        } catch (erro) {
            console.error("Erro ao carregar transações recentes:", erro);
            listaTransacoesEl.innerHTML = `<tr><td colspan="5" class="estado-vazio-pagina">Erro ao carregar transações.</td></tr>`;
        }
    }

    // --- FUNÇÕES DE RENDERIZAÇÃO ---

    function renderizarGrafico(ctx, instance, label, dados, chaveLabel, chaveValor) {
        if (instance) {
            instance.destroy();
        }

        if (!dados || dados.length === 0) {
            ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
            ctx.font = "16px 'Segoe UI'";
            ctx.fillStyle = '#6b7280';
            ctx.textAlign = 'center';
            ctx.fillText(`Sem ${label.toLowerCase()} para exibir.`, ctx.canvas.width / 2, ctx.canvas.height / 2);
            return null; // Retorna nulo se não houver gráfico
        }

        return new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: dados.map(d => d[chaveLabel]),
                datasets: [{
                    label: label,
                    data: dados.map(d => d[chaveValor]),
                    backgroundColor: CONFIG.UTIL.gerarCores(dados.length),
                    borderColor: '#ffffff',
                    borderWidth: 2
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: 'right' }
                }
            }
        });
    }

    function renderizarTransacoes(transacoes) {
        listaTransacoesEl.innerHTML = '';
        if (!transacoes || transacoes.length === 0) {
            listaTransacoesEl.innerHTML = `<tr><td colspan="5" class="estado-vazio-pagina">Nenhuma transação recente.</td></tr>`;
            return;
        }

        transacoes.slice(0, 5).forEach(t => {
            const eReceita = t.tipoMovimentacao.toLowerCase() === 'receita';
            const tipoClasse = eReceita ? 'receita' : 'despesa';
            const valorFormatado = (eReceita ? '+' : '-') + CONFIG.UTIL.formatarMoeda(Math.abs(t.valor));

            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td><span class="selo-status ${tipoClasse}">${t.tipoMovimentacao}</span></td>
                <td>${t.descricao}</td>
                <td>${t.categoria || 'N/A'}</td>
                <td>${CONFIG.UTIL.formatarData(t.dataMovimentacao)}</td>
                <td class="celula-valor ${tipoClasse}">${valorFormatado}</td>
            `;
            listaTransacoesEl.appendChild(tr);
        });
    }
    
    // --- LÓGICA DO MODAL (RESTAURADA) ---

    async function abrirModal(tipo) {
        estado.tipoTransacaoModal = tipo;
        formularioTransacao.reset();
        tituloModal.textContent = tipo === 'receita' ? 'Nova Receita' : 'Nova Despesa';
        grupoMetodoPagamento.style.display = tipo === 'despesa' ? 'block' : 'none';

        // Carrega dados para os selects do modal
        try {
            const [categorias, contas, metodos] = await Promise.all([
                api.buscarCategorias({ idUsuario }),
                api.buscarContas({ idUsuario }),
                tipo === 'despesa' ? api.buscarMetodosPagamento({ idUsuario }) : Promise.resolve([])
            ]);
            estado.categorias = categorias;
            estado.contas = contas;
            estado.metodosPagamento = metodos;

            popularSelect(document.getElementById('categoria'), estado.categorias, 'idCategoria', 'nome', 'Selecione');
            popularSelect(document.getElementById('conta'), estado.contas, 'idConta', 'numeroConta', 'Selecione');
            if (tipo === 'despesa') {
                popularSelect(document.getElementById('metodoPagamento'), estado.metodosPagamento, 'idMetodo', 'nome', 'Selecione');
            }
        } catch (erro) {
            console.error("Erro ao carregar dados para o modal:", erro);
            alert("Não foi possível carregar os dados para o formulário.");
            return;
        }

        fundoModal.classList.add('ativo');
    }

    function fecharModal() {
        fundoModal.classList.remove('ativo');
    }

    async function submeterFormulario(evento) {
        evento.preventDefault();
        const tipo = estado.tipoTransacaoModal;
        
        const dadosCorpo = {
            valor: parseFloat(document.getElementById('valor').value),
            descricao: document.getElementById('descricao').value.trim(),
            dataReceita: tipo === 'receita' ? document.getElementById('data').value : undefined,
            dataDespesa: tipo === 'despesa' ? document.getElementById('data').value : undefined,
        };

        const dadosParams = {
            idUsuario,
            idCategoria: document.getElementById('categoria').value,
            idConta: document.getElementById('conta').value,
            idMetodoPagamento: tipo === 'despesa' ? document.getElementById('metodoPagamento').value : undefined,
        };

        try {
            if (tipo === 'receita') {
                await api.criarReceita(dadosCorpo, dadosParams);
                alert('Receita criada com sucesso!');
            } else {
                await api.criarDespesa(dadosCorpo, dadosParams);
                alert('Despesa criada com sucesso!');
            }
            fecharModal();
            carregarDadosDashboard(); // Recarrega o dashboard para refletir a nova transação
        } catch (erro) {
            alert(`Erro ao salvar: ${erro.message}`);
        }
    }

    function popularSelect(elemento, dados, valorKey, textoKey, placeholder) {
        elemento.innerHTML = `<option value="">${placeholder}</option>`;
        if (Array.isArray(dados)) {
            dados.forEach(item => {
                elemento.innerHTML += `<option value="${item[valorKey]}">${item[textoKey]}</option>`;
            });
        }
    }

    // --- CONFIGURAÇÃO DE EVENTOS ---
    function configurarEventListeners() {
        selecaoPeriodoEl.addEventListener('change', carregarDadosDashboard);
        
        // Botões de ação rápida
        btnNovaReceita.addEventListener('click', () => abrirModal('receita'));
        btnNovaDespesa.addEventListener('click', () => abrirModal('despesa'));

        // Eventos do modal
        btnFecharModal.addEventListener('click', fecharModal);
        btnCancelarModal.addEventListener('click', fecharModal);
        formularioTransacao.addEventListener('submit', submeterFormulario);
        fundoModal.addEventListener('click', (e) => {
            if (e.target === fundoModal) fecharModal();
        });
    }

    // --- INICIALIZAÇÃO ---
    function init() {
        carregarInfoUsuario();
        carregarDadosDashboard();
        configurarEventListeners();
    }

    init();
});
