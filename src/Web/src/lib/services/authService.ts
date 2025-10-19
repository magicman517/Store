import { GATEWAY_API_URL } from "$env/static/private";

export async function refreshAuth(
    fetch: typeof globalThis.fetch,
    refreshToken: string) {
    const response = await fetch(`${GATEWAY_API_URL}/_/api/auth/refresh`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ refresh_token: refreshToken }),
        credentials: 'include'
    });
    return response;
}

export async function login(
    fetch: typeof globalThis.fetch,
) {
    const response = await fetch(`${GATEWAY_API_URL}/_/api/auth`, {
        method: 'GET',
        credentials: 'include'
    });
    console.log(response);
    return response;
}