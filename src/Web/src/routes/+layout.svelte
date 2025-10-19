<script lang="ts">
	import '../app.css';

	import * as Sidebar from '$lib/components/ui/sidebar/index.js';
	import AppSidebar from './app-sidebar.svelte';
	import favicon from '$lib/assets/favicon.svg';

	import { ModeWatcher } from 'mode-watcher';
	import { Toaster } from '@/components/ui/sonner';
	import { ScrollArea } from '$lib/components/ui/scroll-area/index.js';

	let { children } = $props();
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
</svelte:head>

<ModeWatcher />
<Toaster />

<Sidebar.Provider>
	<AppSidebar />
	<Sidebar.Inset>
		<ScrollArea class="grow shrink basis-0 overflow-y-auto">
			<header class="fixed flex flex-row items-center p-2">
				<Sidebar.Trigger />
			</header>
			{@render children?.()}
		</ScrollArea>
	</Sidebar.Inset>
</Sidebar.Provider>
