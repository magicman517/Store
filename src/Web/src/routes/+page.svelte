<script lang="ts">
</script>

<div
	class="grid h-full w-full grid-cols-[repeat(auto-fit,minmax(200px,1fr))] items-start justify-start gap-6 px-6 pt-20 pb-6"
>
	{#each Array(20) as _, index}
		<div class="flex h-40 items-center justify-center rounded-lg bg-gray-700">
			Product Card {index + 1}
		</div>
	{/each}
</div>
