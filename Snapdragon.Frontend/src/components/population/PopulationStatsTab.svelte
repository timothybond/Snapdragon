<script lang="ts">
    import { cardCounts } from "../../stores/stores";
    import CardCountChart from "../CardCountChart.svelte";
</script>

<div>
    <CardCountChart cardCounts={$cardCounts}></CardCountChart>
</div>
