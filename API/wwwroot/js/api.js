import { CONFIG } from './config.js';

// --- Funções Auxiliares para gerenciar o Token de Autenticação ---

const salvarToken = (token) => {
    localStorage.setItem('finon_token', token);
};

const obterToken = () => {
    return localStorage.getItem('finon_token');
};

const removerToken = () => {
    localStorage.removeItem('finon_token');
};


// --- Função Principal e Genérica para Requisições à API (CORRIGIDA E MAIS ROBUSTA) ---

async function requisicaoApi(endpoint, metodo = 'GET', corpo = null) {
    const url = `${CONFIG.API.BASE_URL}${endpoint}`;
    const token = obterToken();

    const opcoes = {
        method: metodo,
        headers: { 'Content-Type': 'application/json' },
    };

    if (token) {
        opcoes.headers['Authorization'] = `Bearer ${token}`;
    }

    if (corpo) {
        opcoes.body = JSON.stringify(corpo);
    }

    try {
        const resposta = await fetch(url, opcoes);

        if (!resposta.ok) {
            const erroData = await resposta.json().catch(() => ({ message: resposta.statusText }));
            throw new Error(erroData.message || 'Ocorreu um erro na comunicação com o servidor.');
        }

        if (resposta.status === 204) { // No Content
            return null;
        }

        // --- LÓGICA CORRIGIDA E MAIS SEGURA ---
        // Clona a resposta para poder ler o corpo como texto sem consumir o stream original.
        const respostaTexto = await resposta.clone().text();

        // Se o corpo da resposta estiver vazio, retorna um valor padrão seguro.
        if (!respostaTexto) {
            // Para endpoints que deveriam retornar listas, um array vazio é o mais seguro.
            // Para o endpoint de total que retorna um número, retornamos 0.
            return endpoint.includes('/total/') ? '0' : [];
        }

        // Se o corpo não estiver vazio, processa de acordo com o tipo de conteúdo.
        const contentType = resposta.headers.get("content-type");
        if (contentType && contentType.includes("application/json")) {
            return resposta.json();
        } else {
            return resposta.text();
        }

    } catch (erro) {
        console.error(`Erro na requisição para ${url}:`, erro.message);
        throw erro;
    }
}


// --- "Cardápio" de Funções da API (sem alterações, apenas usando a nova função de requisição) ---

export const api = {
    // --- API de Autenticação e Usuário ---
    login: async (credenciais) => {
        const dados = await requisicaoApi('/usuario/login', 'POST', credenciais);
        if (dados && dados.token) {
            salvarToken(dados.token);
            localStorage.setItem('finon_user_id', dados.idUsuario);
        }
        return dados;
    },
    logout: () => {
        removerToken();
        localStorage.removeItem('finon_user_id');
    },
    cadastrarUsuario: (dadosUsuario) => {
        return requisicaoApi('/cliente/dados', 'POST', dadosUsuario);
    },
    buscarDadosPessoais: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/cliente/dados?${queryString}`, 'GET');
    },
    atualizarDadosPessoais: (dadosUsuario, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/cliente/dados?${queryString}`, 'PUT', dadosUsuario);
    },
    deletarUsuario: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/cliente/dados?${queryString}`, 'DELETE');
    },

    // --- API de Categorias ---
    buscarCategorias: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/categorias/cliente?${queryString}`, 'GET');
    },
    criarCategoria: (dadosCategoria, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/categoria/cliente?${queryString}`, 'POST', dadosCategoria);
    },
    atualizarCategoria: (dadosCategoria, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/categoria/cliente?${queryString}`, 'PUT', dadosCategoria);
    },
    deletarCategoria: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/categoria/cliente?${queryString}`, 'DELETE');
    },

    // --- API de Contas ---
    buscarContas: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/contas/cliente?${queryString}`, 'GET');
    },
    criarConta: (dadosConta, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/conta/cliente?${queryString}`, 'POST', dadosConta);
    },
    atualizarConta: (dadosConta, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/conta/cliente?${queryString}`, 'PUT', dadosConta);
    },
    deletarConta: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/conta/cliente?${queryString}`, 'DELETE');
    },
    
    // --- API de Despesas ---
    buscarDespesas: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/despesas/cliente?${queryString}`, 'GET');
    },
    criarDespesa: (dadosDespesa, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/despesa/cliente?${queryString}`, 'POST', dadosDespesa);
    },
    atualizarDespesa: (dadosDespesa, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/despesa/cliente?${queryString}`, 'PUT', dadosDespesa);
    },
    deletarDespesa: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/despesa/cliente?${queryString}`, 'DELETE');
    },
    buscarDespesasPorCategoria: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/despesas/categoria/cliente?${queryString}`, 'GET');
    },
    buscarDespesasPorConta: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/despesas/conta/cliente?${queryString}`, 'GET');
    },
    buscarDespesasPorMetodoPagamento: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/despesas/metodo/cliente?${queryString}`, 'GET');
    },
    buscarTotalDespesas: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/despesas/total/cliente?${queryString}`, 'GET');
    },

    // --- API de Receitas ---
    buscarReceitas: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/receitas/cliente?${queryString}`, 'GET');
    },
    criarReceita: (dadosReceita, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/receita/cliente?${queryString}`, 'POST', dadosReceita);
    },
    atualizarReceita: (dadosReceita, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/receita/cliente?${queryString}`, 'PUT', dadosReceita);
    },
    deletarReceita: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/receita/cliente?${queryString}`, 'DELETE');
    },
    buscarReceitasPorCategoria: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/receitas/categoria/cliente?${queryString}`, 'GET');
    },
    buscarReceitasPorConta: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/receitas/conta/cliente?${queryString}`, 'GET');
    },
    buscarTotalReceitas: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/receitas/total/cliente?${queryString}`, 'GET');
    },

    // --- API de Metas Financeiras ---
    buscarMetas: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/metas/cliente?${queryString}`, 'GET');
    },
    criarMeta: (dadosMeta, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/meta/cliente?${queryString}`, 'POST', dadosMeta);
    },
    atualizarMeta: (dadosMeta, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/meta/cliente?${queryString}`, 'PUT', dadosMeta);
    },
    deletarMeta: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/meta/cliente?${queryString}`, 'DELETE');
    },

    // --- API de Métodos de Pagamento ---
    buscarMetodosPagamento: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/metodospagamento?${queryString}`, 'GET');
    },
    criarMetodoPagamento: (dadosMetodo, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/metodopagamento?${queryString}`, 'POST', dadosMetodo);
    },
    atualizarMetodoPagamento: (dadosMetodo, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/metodopagamento?${queryString}`, 'PUT', dadosMetodo);
    },
    deletarMetodoPagamento: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/metodopagamento?${queryString}`, 'DELETE');
    },

    // --- API de Movimentações Financeiras (Transferências) ---
    buscarMovimentacoes: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/movimentacoes/cliente?${queryString}`, 'GET');
    },
    criarMovimentacao: (dadosMovimentacao, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/movimentacao/cliente?${queryString}`, 'POST', dadosMovimentacao);
    },
    atualizarMovimentacao: (dadosMovimentacao, params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/movimentacao/cliente?${queryString}`, 'PUT', dadosMovimentacao);
    },
    deletarMovimentacao: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/movimentacao/cliente?${queryString}`, 'DELETE');
    },

    // --- API de Saldos ---
    buscarSaldoPorConta: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/saldo/contas/cliente?${queryString}`, 'GET');
    },
    buscarSaldoTotal: (params) => {
        const queryString = new URLSearchParams(params).toString();
        return requisicaoApi(`/saldo/total/cliente?${queryString}`, 'GET');
    },
};
