import type { LayoutServerLoad } from './$types';

export const load: LayoutServerLoad = async ({ fetch, cookies }) => {
	// TODO: load categories and store in global state
	// TODO: load user profile if access token is valid and store in global state
	// const accessToken = cookies.get('SECURE_access_token');
	//
	// for global state use classes and sveltekit's context
};
