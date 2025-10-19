import type { Handle } from '@sveltejs/kit';
import { paraglideMiddleware } from '$lib/paraglide/server';
import { sequence } from '@sveltejs/kit/hooks';
import { refreshAuth } from '@/services/authService';

const handleParaglide: Handle = ({ event, resolve }) =>
	paraglideMiddleware(event.request, ({ request, locale }) => {
		event.request = request;

		return resolve(event, {
			transformPageChunk: ({ html }) => html.replace('%paraglide.lang%', locale)
		});
	});

const handleAuth: Handle = async ({ event, resolve }) => {
	const accessToken = event.cookies.get('SECURE_access_token');
	if (accessToken) {
		return await resolve(event);
	}

	const refreshToken = event.cookies.get('SECURE_refresh_token');
	if (refreshToken) {
		const response = await refreshAuth(event.fetch, refreshToken);
	}

	return await resolve(event);
};

export const handle: Handle = sequence(handleParaglide, handleAuth);
