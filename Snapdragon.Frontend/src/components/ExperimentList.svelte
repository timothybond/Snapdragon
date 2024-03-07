<script lang="ts">
    import SvelteTable, { type TableColumn } from "svelte-table";
    import { experiments, selectedExperimentId } from "../stores/stores";
    import type { Experiment } from "../types/models";

    experiments.refresh();

    const columns: TableColumn<Experiment>[] = [
        {
            key: "id",
            title: "ID",
            value: (e) => e.id,
            sortable: true,
        },
        { key: "name", title: "Name", value: (e) => e.name, sortable: true },
        {
            key: "started",
            title: "Started",
            value: (e) => e.started,
            sortable: true,
        },
    ];

    function clickTableCell(
        e: CustomEvent<{
            event: PointerEvent;
            row: Experiment;
            key: string | number;
        }>
    ) {
        selectedExperimentId.set(e.detail.row.id);
    }
</script>

<div>
    <SvelteTable
        {columns}
        rows={$experiments}
        classNameTable="border table-w-auto"
        classNameCell="border px-2"
        classNameThead="bg-slate-300"
        on:clickCell={clickTableCell}
    ></SvelteTable>
</div>

<style>
</style>
