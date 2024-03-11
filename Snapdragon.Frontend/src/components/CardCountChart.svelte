<script lang="ts">
    import { Chart } from "chart.js";
    import type { CardCounts } from "../types/models";

    export let cardCounts: CardCounts[];

    let canvas: HTMLCanvasElement;
    let chart: Chart<"line", number[]> | undefined = undefined;

    $: datasets = cardCounts.map((cc) => {
        return {
            data: cc.counts,
            label: cc.name,
        };
    });

    $: labels = cardCounts[0]?.counts.map((v, i) => i) ?? [];

    $: {
        if (chart) {
            chart.data = {
                labels: labels,
                datasets: datasets,
            };
            chart.update();
        }
    }

    function addChart(node: HTMLCanvasElement) {
        chart = new Chart(node, {
            type: "line",
            data: {
                labels: labels,
                datasets: datasets,
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true,
                    },
                    x: {},
                },
                plugins: {
                    legend: {
                        display: false,
                    },
                },
            },
        });

        return {
            destroy() {
                chart?.destroy();
            },
        };
    }
</script>

<div>
    <canvas bind:this={canvas} use:addChart></canvas>
</div>
