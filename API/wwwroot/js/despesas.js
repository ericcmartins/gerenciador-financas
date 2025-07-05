import { api } from './api.js';
import { CONFIG } from './config.js';

document.addEventListener('DOMContentLoaded', () => {
    const idUsuario = localStorage.getItem('finon_user_id');
    if (!idUsuario) {
        // A guarda em main.js já deve ter redirecionado, mas é uma segurança extra.
        window.location.href = 'login.html';
        return;
    }

    // Objeto para guardar o estado da página (dados, filtros, etc.)
    const estado = {
        despesas: [],
        categorias: [],
        contas: [],
        metodosPagamento: [],
        despesaParaEditar: null,
        despesaParaDeletar: null,
    };

    // --- SELEÇÃO DE ELEMENTOS DO DOM ---
    const corpoTabela = document.getElementById('corpoTabelaDespesas');
    const totalDespesasEl = document.getElementById('totalDespesas');
    
    // Filtros
    const filtroDataEl = document.getElementById('filtroData');
    const filtroCategoriaEl = document.getElementById('filtroCategoria');
    const filtroContaEl = document.getElementById('filtroConta');
    const filtroMetodoEl = document.getElementById('filtroMetodo');
    const btnAplicarFiltros = document.getElementById('btnAplicarFiltros');
    
    // Modal
    const fundoModal = document.getElementById('fundoModal');
    const modalDespesa = document.getElementById('modalDespesa');
    const tituloModal = document.getElementById('tituloModal');
    const formularioDespesa = document.getElementById('formularioDespesa');
    const btnNovaDespesa = document.getElementById('btnNovaDespesa');
    const btnFecharModal = document.getElementById('fecharModal');
    const btnCancelarModal = document.getElementById('btnCancelarModal');

    // Campos do formulário do modal
    const inputIdDespesa = new_form_input_that_I_will_add_to_your_html_to_handle_updates;
    const inputValor = document.getElementById('valor');
    const inputData = document.getElementById('dataDespesa');
    const inputDescricao = document.getElementById('descricao');
    const selectCategoria = document.getElementById('categoria');
    const selectConta = document.getElementById('conta');
    const selectMetodo = document.getElementById('metodoPagamento');


    // --- FUNÇÕES DE CARREGAMENTO E RENDERIZAÇÃO ---

    /**
     * Busca os dados iniciais (categorias, contas, etc.) para popular os filtros e o formulário.
     */
    async function carregarDadosDeSuporte() {
        try {
            const [categorias, contas, metodos] = await Promise.all([
                api.buscarCategorias({ idUsuario }),
                api.buscarContas({ idUsuario }),
                api.buscarMetodosPagamento({ idUsuario })
            ]);

            estado.categorias = categorias;
            estado.contas = contas;
            estado.metodosPagamento = metodos;

            popularSelect(filtroCategoriaEl, categorias, 'nome', 'idCategoria');
            popularSelect(filtroContaEl, contas, 'numeroConta', 'idConta');
            popularSelect(filtroMetodoEl, metodos, 'nome', 'idMetodo');

            popularSelect(selectCategoria, categorias, 'nome', 'idCategoria');
            popularSelect(selectConta, contas, 'numeroConta', 'idConta');
            popularSelect(selectMetodo, metodos, 'nome', 'idMetodo');

        } catch (erro) {
            console.error("Erro ao carregar dados de suporte:", erro.message);
            alert("Não foi possível carregar os dados para os filtros. Tente recarregar a página.");
        }
    }

    /**
     * Busca as despesas na API com base nos filtros selecionados e atualiza a tela.
     */
    async function carregarDespesas() {
        corpoTabela.innerHTML = `<tr><td colspan="8" class="carregando">Carregando despesas...</td></tr>`;

        try {
            const dias = converterPeriodoParaDias(filtroDataEl.value);
            
            const filtros = {
                idUsuario,
                periodo: dias,
                idCategoria: filtroCategoriaEl.value,
                idConta: filtroContaEl.value,
                idMetodoPagamento: filtroMetodoEl.value,
            };

            // Remove filtros vazios para não enviar na URL
            Object.keys(filtros).forEach(key => (!filtros[key]) && delete filtros[key]);

            const despesas = await api.buscarDespesas(filtros);
            estado.despesas = despesas;
            renderizarTabela();
            atualizarResumo();

        } catch (erro) {
            console.error("Erro ao carregar despesas:", erro.message);
            corpoTabela.innerHTML = `<tr><td colspan="8" class="erro">Erro ao carregar despesas.</td></tr>`;
        }
    }

    /**
     * Desenha as linhas da tabela com os dados de despesas.
     */
    function renderizarTabela() {
        corpoTabela.innerHTML = '';
        if (estado.despesas.length === 0) {
            corpoTabela.innerHTML = `<tr><td colspan="8" class="estado-vazio-pagina">Nenhuma despesa encontrada.</td></tr>`;
            return;
        }

        estado.despesas.forEach(despesa => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${CONFIG.UTIL.formatarData(despesa.dataDespesa)}</td>
                <td>${despesa.descricao}</td>
                <td>${despesa.categoria || 'N/A'}</td>
                <td>${despesa.conta || 'N/A'}</td>
                <td>${despesa.metodoPagamento || 'N/A'}</td>
                <td class="celula-valor despesa">${CONFIG.UTIL.formatarMoeda(despesa.valor)}</td>
                <td><span class="selo-status ${despesa.recorrente ? 'recorrente' : ''}">${despesa.recorrente ? 'Sim' : 'Não'}</span></td>
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

    /**
     * Atualiza os cards de resumo com base nas despesas carregadas.
     */
    function atualizarResumo() {
        const total = estado.despesas.reduce((acc, despesa) => acc + despesa.valor, 0);
        totalDespesasEl.textContent = CONFIG.UTIL.formatarMoeda(total);
        // Lógica para despesas recorrentes e média mensal pode ser adicionada aqui
    }

    
    // --- LÓGICA DO MODAL E FORMULÁRIO ---

    function abrirModal(despesa = null) {
        formularioDespesa.reset();
        inputIdDespesa.value = '';

        if (despesa) { // Modo Edição
            tituloModal.textContent = 'Editar Despesa';
            estado.despesaParaEditar = despesa;
            
            // Preenche o formulário
            inputIdDespesa.value = despesa.idDespesa;
            inputValor.value = despesa.valor;
            inputDescricao.value = despesa.descricao;
            inputData.value = CONFIG.UTIL.formatarDataParaInput(despesa.dataDespesa);
            selectCategoria.value = estado.categorias.find(c => c.nome === despesa.categoria)?.idCategoria || '';
            selectConta.value = estado.contas.find(c => c.numeroConta === despesa.conta)?.idConta || '';
            selectMetodo.value = estado.metodosPagamento.find(m => m.nome === despesa.metodoPagamento)?.idMetodo || '';
        } else { // Modo Criação
            tituloModal.textContent = 'Nova Despesa';
            estado.despesaParaEditar = null;
        }
        fundoModal.classList.add('ativo');
    }

    function fecharModal() {
        fundoModal.classList.remove('ativo');
    }

    async function submeterFormulario(evento) {
        evento.preventDefault();
        const id = inputIdDespesa.value; // Se tiver um ID, é uma edição
        
        const dadosCorpo = {
            valor: parseFloat(inputValor.value),
            descricao: inputDescricao.value.trim(),
            dataDespesa: inputData.value,
        };
        
        const dadosParams = {
            idUsuario,
            idCategoria: selectCategoria.value,
            idConta: selectConta.value,
            idMetodoPagamento: selectMetodo.value,
        };

        try {
            if (id) { // ATUALIZAR (PUT)
                dadosParams.idDespesa = id;
                await api.atualizarDespesa(dadosCorpo, dadosParams);
                alert('Despesa atualizada com sucesso!');
            } else { // CRIAR (POST)
                await api.criarDespesa(dadosCorpo, dadosParams);
                alert('Despesa criada com sucesso!');
            }
            fecharModal();
            carregarDespesas(); // Recarrega a tabela
        } catch (erro) {
            console.error("Erro ao salvar despesa:", erro.message);
            alert(`Erro ao salvar: ${erro.message}`);
        }
    }


    // --- FUNÇÕES AUXILIARES GERAIS ---
    
    function popularSelect(selectElement, dados, textoProp, valorProp) {
        // Limpa opções antigas, mantendo a primeira ("Todos" ou "Selecione")
        selectElement.length = 1; 
        dados.forEach(item => {
            const option = new Option(item[textoProp], item[valorProp]);
            selectElement.add(option);
        });
    }

    function converterPeriodoParaDias(valorPeriodo) {
        const hoje = new Date();
        switch (valorPeriodo) {
            case '1': return hoje.getDate();
            case '3': return 90;
            case '6': return 180;
            case '12':
                const inicioDoAno = new Date(hoje.getFullYear(), 0, 1);
                return Math.ceil((hoje - inicioDoAno) / (1000 * 60 * 60 * 24));
            default: return 30;
        }
    }


    // --- CONFIGURAÇÃO DOS EVENT LISTENERS ---
    
    function configurarEventListeners() {
        btnAplicarFiltros.addEventListener('click', carregarDespesas);
        btnNovaDespesa.addEventListener('click', () => abrirModal());
        btnFecharModal.addEventListener('click', fecharModal);
        btnCancelarModal.addEventListener('click', fecharModal);
        formularioDespesa.addEventListener('submit', submeterFormulario);

        corpoTabela.addEventListener('click', (evento) => {
            const botao = evento.target.closest('button.botao-acao');
            if (!botao) return;

            const id = botao.dataset.id;
            if (botao.classList.contains('editar')) {
                const despesa = estado.despesas.find(d => d.idDespesa == id);
                abrirModal(despesa);
            }
            if (botao.classList.contains('deletar')) {
                // Lógica de deleção aqui...
                if (confirm('Tem certeza que deseja excluir esta despesa?')) {
                    api.deletarDespesa({idUsuario, idDespesa: id})
                       .then(() => {
                           alert('Despesa excluída com sucesso!');
                           carregarDespesas();
                       })
                       .catch(err => alert(`Erro ao excluir: ${err.message}`));
                }
            }
        });
    }

    // --- INICIALIZAÇÃO DA PÁGINA ---
    
    async function init() {
        await carregarDadosDeSuporte();
        await carregarDespesas();
        configurarEventListeners();
    }

    init();
});