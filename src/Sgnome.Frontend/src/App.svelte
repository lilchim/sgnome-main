<script lang="ts">
  import GraphView from './lib/components/GraphView.svelte';
  import StateTest from './lib/components/StateTest.svelte';
  
  let showDebug = $state(false);
</script>

<div class="app">
  <!-- Top Bar -->
  <header class="top-bar">
    <h1>Sgnome</h1>
    <button class="debug-btn" on:click={() => showDebug = !showDebug}>
      {showDebug ? 'Hide' : 'Show'} Debug
    </button>
  </header>
  
  <!-- Main Content -->
  <main class="main-content">
    <GraphView />
  </main>
  
  <!-- Debug Overlay -->
  {#if showDebug}
    <div class="debug-overlay">
      <StateTest />
    </div>
  {/if}
</div>

<style>
  /* Global styles */
  * {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
  }

  body {
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
    height: 100vh;
    overflow: hidden;
  }

  .app {
    height: 100vh;
    display: flex;
    flex-direction: column;
  }

  .top-bar {
    height: 50px;
    background: #f5f5f5;
    border-bottom: 1px solid #ddd;
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0 16px;
    flex-shrink: 0;
  }

  .top-bar h1 {
    font-size: 18px;
    font-weight: 600;
  }

  .debug-btn {
    padding: 6px 12px;
    background: #007acc;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 12px;
  }

  .debug-btn:hover {
    background: #005a9e;
  }

  .main-content {
    flex: 1;
    overflow: hidden;
  }

  .debug-overlay {
    position: fixed;
    top: 60px;
    right: 16px;
    width: 300px;
    max-height: 400px;
    background: white;
    border: 1px solid #ddd;
    border-radius: 4px;
    box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    z-index: 1000;
    overflow: auto;
  }
</style>
