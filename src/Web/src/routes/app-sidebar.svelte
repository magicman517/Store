<script lang="ts">
	import * as Sidebar from '$lib/components/ui/sidebar';
	import * as DropdownMenu from '$lib/components/ui/dropdown-menu';
	import * as ButtonGroup from '$lib/components/ui/button-group';

	import { Anvil, LogInIcon, Search, UserRound } from '@lucide/svelte';
	import { Button } from '@/components/ui/button';
	import { Separator } from '@/components/ui/separator';

	import { m } from '$lib/paraglide/messages.js';
	import { setLocale, getLocale } from '$lib/paraglide/runtime';

	async function onLogin() {
		const response = await fetch('/api/auth');
		if (response.status != 200) return;

		const redirectUrl = response.url;
		if (redirectUrl) window.location.href = redirectUrl;
	}
</script>

<Sidebar.Root variant="inset">
	{@render header()}
	{@render content()}
	{@render footer()}
</Sidebar.Root>

{#snippet header()}
	<Sidebar.Header>
		<Sidebar.Menu class="flex flex-col gap-4">
			<Sidebar.MenuItem>
				<Sidebar.MenuButton size="lg">
					{#snippet child({ props })}
						<a href="/" {...props}>
							<Anvil />
							<span class="text-lg">My App</span>
						</a>
					{/snippet}
				</Sidebar.MenuButton>
			</Sidebar.MenuItem>

			<!-- TODO: use command or dialog for search -->
			<Sidebar.MenuItem>
				<Sidebar.MenuButton variant="outline">
					{#snippet child({ props })}
						<button {...props} onclick={() => console.log('Search clicked')}>
							<Search />
							<span>{m['sidebar.search']()}</span>
						</button>
					{/snippet}
				</Sidebar.MenuButton>
			</Sidebar.MenuItem>
		</Sidebar.Menu>
	</Sidebar.Header>
{/snippet}

{#snippet content()}
	<Sidebar.Content>
		<Sidebar.Menu class="px-1.5">
			<Sidebar.MenuItem>
				<Sidebar.MenuButton>
					{#snippet child({ props })}
						<a href="/profile" {...props}>
							<UserRound />
							<span>{m['sidebar.myProfile']()}</span>
						</a>
					{/snippet}
				</Sidebar.MenuButton>
			</Sidebar.MenuItem>
		</Sidebar.Menu>
	</Sidebar.Content>
{/snippet}

{#snippet footer()}
	<Sidebar.Footer>
		<Sidebar.Menu>
			<Sidebar.MenuItem>
				<DropdownMenu.Root>
					<Sidebar.MenuButton
						class="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground"
					>
						{#snippet child({ props })}
							<DropdownMenu.Trigger {...props}>
								{#if getLocale() === 'uk'}
									🇺🇦 Українська
								{:else}
									🇬🇧 English
								{/if}
							</DropdownMenu.Trigger>
							<DropdownMenu.Content class="w-(--bits-dropdown-menu-anchor-width)" side="top">
								<ButtonGroup.Root
									orientation="vertical"
									aria-label="Select Language"
									class="w-full"
								>
									<Button variant="ghost" onclick={() => setLocale('uk')}>🇺🇦 Українська</Button>
									<Separator />
									<Button variant="ghost" onclick={() => setLocale('en')}>🇬🇧 English</Button>
								</ButtonGroup.Root>
							</DropdownMenu.Content>
						{/snippet}
					</Sidebar.MenuButton>
				</DropdownMenu.Root>
			</Sidebar.MenuItem>
			<Sidebar.MenuItem>
				<Sidebar.MenuButton variant="outline" size="lg">
					{#snippet child({ props })}
						<button {...props} onclick={onLogin}>
							<LogInIcon />
							<span>{m['sidebar.login']()}</span>
						</button>
					{/snippet}
				</Sidebar.MenuButton>
			</Sidebar.MenuItem>
		</Sidebar.Menu>
	</Sidebar.Footer>
{/snippet}
