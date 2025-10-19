import { login } from "@/services/authService";
import type { RequestHandler } from "@sveltejs/kit";
import { redirect } from "@sveltejs/kit";

export const GET: RequestHandler = async (event) => {
    const response = await login(event.fetch);

    const authUrl = response.headers.get('Location');
    if (authUrl) {
        throw redirect(303, authUrl);
    }

    return new Response('Unable to determine authentication url', { status: 500 });
};