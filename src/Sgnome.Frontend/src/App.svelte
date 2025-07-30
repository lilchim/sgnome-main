<script lang="ts">
  import './app.css';
  import GraphView from './lib/components/GraphView.svelte';
  import StateTest from './lib/components/StateTest.svelte';
  
  let showDebug = $state(false);
  let isDark = $state(true); // Default to dark theme
  
  // Apply dark class to document
  $effect(() => {
    if (isDark) {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }
  });
</script>

<div class="app">
  <!-- Top Bar -->
  <header class="top-bar">
    <h1>Sgnome</h1>
    <div class="flex items-center gap-2">
      <button class="theme-btn" on:click={() => isDark = !isDark}>
        {isDark ? '☀️' : '🌙'} {isDark ? 'Light' : 'Dark'}
      </button>
      <button class="debug-btn" on:click={() => showDebug = !showDebug}>
        {showDebug ? 'Hide' : 'Show'} Debug
      </button>
    </div>
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
    background: var(--background);
    border-bottom: 1px solid var(--border);
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0 16px;
    flex-shrink: 0;
  }

  .top-bar h1 {
    font-size: 18px;
    font-weight: 600;
    color: var(--foreground);
  }

  .debug-btn, .theme-btn {
    padding: 6px 12px;
    background: var(--primary);
    color: var(--primary-foreground);
    border: 1px solid var(--border);
    border-radius: var(--radius);
    cursor: pointer;
    font-size: 12px;
    transition: all 0.2s;
  }

  .debug-btn:hover, .theme-btn:hover {
    background: var(--primary);
    opacity: 0.9;
  }

  .main-content {
    flex: 1;
    overflow: hidden;
    background: var(--background);
  }

  .debug-overlay {
    position: fixed;
    top: 60px;
    right: 16px;
    width: 300px;
    max-height: 400px;
    background: var(--card);
    border: 1px solid var(--border);
    border-radius: var(--radius);
    box-shadow: var(--xy-node-shadow);
    z-index: 1000;
    overflow: auto;
    color: var(--card-foreground);
  }
</style>
