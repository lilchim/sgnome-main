<script lang="ts">
  import './app.css';
  import '@xyflow/svelte/dist/style.css';
  import * as Sidebar from './lib/components/ui/sidebar/index.js';
  import AppSidebar from './lib/components/AppSidebar.svelte';
  import GraphView from './lib/components/GraphView.svelte';
  import StateTest from './lib/components/StateTest.svelte';
    import SiteHeader from '$lib/components/SiteHeader.svelte';
  
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

<Sidebar.Provider>
  <AppSidebar />
  <Sidebar.Inset>
  <main class="main-layout">
    <SiteHeader />
    
    <div class="content-area">
      <GraphView />
    </div>
    
    <!-- Debug Overlay -->
    {#if showDebug}
      <div class="debug-overlay">
        <StateTest />
      </div>
    {/if}
  </main>
  </Sidebar.Inset>
</Sidebar.Provider>

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

  .main-layout {
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

  .content-area {
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
