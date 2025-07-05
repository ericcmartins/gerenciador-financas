import { api } from './api.js';
import { CONFIG } from './config.js';

document.addEventListener('DOMContentLoaded', () => {
    const idUsuario = localStorage.getItem('finon_user_id');
    if (!idUsuario) return;

    // --- Seleção de Elementos ---
    const elementoTotalReceitas = document.getElementById('totalReceitas');
    const elementoTotalDespesas = document.getElementById('totalDespesas');
    const elementoSaldoTotal = document.getElementById('saldoTotal');
    const elementoMetasAtivas = document.getElementById('metasAtivas');
    const listaTransacoes = document.getElementById('listaTransacoes');
    const selecaoPeriodo = document.getElementById('selecaoPeriodo');
    
    // Variável para guardar a instância do gráfico e poder atualizá-la
    let graficoDespesasInstance = null;

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


    // --- Funções de Carregamento de Dados ---

    async function carregarResumo() {
        try {
            const diasEsteMes = converterPeriodoParaDias('1');
            const [dadosReceitas, dadosDespesas, dadosSaldo, dadosMetas] = await Promise.all([
                api.buscarReceitas({ idUsuario, periodo: diasEsteMes }),
                api.buscarDespesas({ idUsuario, periodo: diasEsteMes }),
                api.buscarSaldoTotal({ idUsuario }),
                api.buscarMetas({ idUsuario })
            ]);

            const totalReceitas = dadosReceitas.reduce((acc, r) => acc + r.valor, 0);
            elementoTotalReceitas.textContent = CONFIG.UTIL.formatarMoeda(totalReceitas);

            const totalDespesas = dadosDespesas.reduce((acc, d) => acc + d.valor, 0);
            elementoTotalDespesas.textContent = CONFIG.UTIL.formatarMoeda(totalDespesas);

            const saldo = dadosSaldo.length > 0 ? dadosSaldo[0].saldoTotal : 0;
            elementoSaldoTotal.textContent = CONFIG.UTIL.formatarMoeda(saldo);

            const metasAtivas = dadosMetas.filter(m => !m.concluida).length;
            elementoMetasAtivas.textContent = metasAtivas;

        } catch (erro) {
            console.error('Falha ao carregar resumo do dashboard:', erro.message);
        }
    }

    async function carregarGraficoDespesas() {
        try {
            const valorPeriodoSelecionado = selecaoPeriodo.value;
            const dias = converterPeriodoParaDias(valorPeriodoSelecionado);

            const dados = await api.buscarDespesasPorCategoria({ idUsuario, periodo: dias });
            const ctx = document.getElementById('graficoDespesas').getContext('2d');

            // Se já existe uma instância do gráfico, destrói antes de criar uma nova.
            if (graficoDespesasInstance) {
                graficoDespesasInstance.destroy();
            }

            graficoDespesasInstance = new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: dados.map(d => d.categoria),
                    datasets: [{
                        label: 'Despesas por Categoria',
                        data: dados.map(d => d.totalDespesa),
                        backgroundColor: ['#ef4444', '#f97316', '#f59e0b', '#eab308', '#84cc16', '#22c55e', '#10b981', '#14b8a6', '#06b6d4', '#0ea5e9', '#3b82f6', '#6366f1'],
                        borderColor: '#ffffff',
                        borderWidth: 2
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: { legend: { position: 'right' } }
                }
            });
        } catch (erro) {
            console.error('Falha ao carregar gráfico de despesas:', erro.message);
        }
    }

    async function carregarTransacoesRecentes() {
        // (Esta função permanece a mesma da versão anterior)
        try {
            const transacoes = await api.buscarMovimentacoes({ idUsuario });
            listaTransacoes.innerHTML = ''; 

            if (transacoes.length === 0) {
                listaTransacoes.innerHTML = `<div class="estado-vazio"><p>Nenhuma transação encontrada.</p></div>`;
                return;
            }

            const transacoesRecentes = transacoes.slice(0, 5);

            transacoesRecentes.forEach(transacao => {
                const tipo = transacao.valor > 0 ? 'receita' : 'despesa';
                const iconeClasse = tipo === 'receita' ? 'fa-arrow-up' : 'fa-arrow-down';
                const valorClasse = tipo === 'receita' ? 'receita' : 'despesa';

                const itemHtml = `
                    <div class="item-transacao">
                        <div class="info-transacao">
                            <div class="icone-transacao ${valorClasse}"> <i class="fas ${iconeClasse}"></i> </div>
                            <div class="detalhes-transacao">
                                <h4>${transacao.descricao}</h4>
                                <p>${transacao.numeroContaOrigem || 'N/A'}</p>
                            </div>
                        </div>
                        <div class="valor-transacao">
                            <p class="valor ${valorClasse}">${CONFIG.UTIL.formatarMoeda(transacao.valor)}</p>
                            <p class="data">${CONFIG.UTIL.formatarData(transacao.dataMovimentacao)}</p>
                        </div>
                    </div>
                `;
                listaTransacoes.innerHTML += itemHtml;
            });
        } catch (erro) {
            console.error('Falha ao carregar transações recentes:', erro.message);
            listaTransacoes.innerHTML = `<div class="estado-vazio"><p>Erro ao carregar transações.</p></div>`;
        }
    }
    
    // (Lógica do Modal permanece a mesma)
    const fundoModal = document.getElementById('fundoModal');
    // ... (resto da lógica do modal que já tínhamos) ...


    // --- Event Listeners ---
    
    // Adiciona o "ouvinte" para o seletor de período.
    // Sempre que o valor mudar, a função para carregar o gráfico será chamada novamente.
    selecaoPeriodo.addEventListener('change', carregarGraficoDespesas);


    // --- Inicialização ---
    carregarResumo();
    carregarGraficoDespesas(); // Carrega o gráfico com o período inicial
    carregarTransacoesRecentes();
});
