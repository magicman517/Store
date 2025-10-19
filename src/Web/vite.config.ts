import tailwindcss from '@tailwindcss/vite';

import { paraglideVitePlugin } from '@inlang/paraglide-js';
import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';

export default defineConfig({
	plugins: [
		tailwindcss(),
		sveltekit(),
		paraglideVitePlugin({
			project: './project.inlang',
			outdir: './src/lib/paraglide'
		})
	],
	server: {
		port: process.env.PORT ? Number(process.env.PORT) : 5173,
		allowedHosts: ['host.docker.internal']
	}
});
