<script lang="ts">
  import { createEventDispatcher } from 'svelte';
  import type { NodeData } from '../types/graph';
  import { PinBehavior, PinState } from '../types/graph';

  export let data: NodeData;
  export let id: string; // The node ID from SvelteFlow

  const dispatch = createEventDispatcher<{
    expandPin: { nodeId: string; pinId: string };
  }>();

  // Handle pin expansion
  function handlePinClick(pinId: string) {
    // Only allow expansion for expandable pins that aren't already expanded
    const pin = data.pins.find(p => p.id === pinId);
    if (pin && pin.behavior === PinBehavior.Expandable && pin.state === PinState.Unexpanded) {
      // Dispatch a global event for pin expansion
      const event = new CustomEvent('expandPin', {
        detail: { nodeId: id, pinId },
        bubbles: true
      });
      document.dispatchEvent(event);
    }
  }

  // Get icon display
  function getIconDisplay(icon: string | undefined): string {
    if (!icon) return '📌';
    
    const iconMap: Record<string, string> = {
      'steam': '🎮',
      'library': '📚',
      'eye': '👁️',
      'clock': '⏰',
      'player': '👤'
    };
    
    return iconMap[icon] || '📌';
  }

  // Get pin state display
  function getPinStateDisplay(pin: any): string {
    switch (pin.state) {
      case PinState.Loading:
        return '⏳ Loading...';
      case PinState.Expanded:
        return '✅ Loaded';
      default:
        return pin.behavior === PinBehavior.Expandable ? '▶️ Expand' : 'ℹ️ Info';
    }
  }

  // Check if pin is clickable
  function isPinClickable(pin: any): boolean {
    return pin.behavior === PinBehavior.Expandable && pin.state === PinState.Unexpanded;
  }
</script>

<div class="custom-node">
  <!-- Node Header -->
  <div class="node-header">
    <div class="node-label">{data.label}</div>
    <div class="node-type">{data.nodeType}</div>
  </div>

  <!-- Pins Container -->
  <div class="pins-container">
    {#each data.pins as pin (pin.id)}
      <button 
        type="button"
        class="pin-item {isPinClickable(pin) ? 'clickable' : ''} {pin.state === PinState.Loading ? 'loading' : ''}"
        on:click={() => handlePinClick(pin.id)}
        disabled={!isPinClickable(pin)}
      >
        <div class="pin-header">
          <span class="pin-icon">{getIconDisplay(pin.summary.icon)}</span>
          <span class="pin-label">{pin.label}</span>
          {#if pin.summary.count !== undefined}
            <span class="pin-count">({pin.summary.count})</span>
          {/if}
        </div>
        
        <div class="pin-summary">
          {pin.summary.displayText}
        </div>
        
        <div class="pin-action">
          {getPinStateDisplay(pin)}
        </div>
      </button>
    {/each}
  </div>
</div>

<style>
  .custom-node {
    background: white;
    border: 2px solid #e0e0e0;
    border-radius: 8px;
    padding: 12px;
    min-width: 200px;
    max-width: 300px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  }

  .node-header {
    border-bottom: 1px solid #f0f0f0;
    padding-bottom: 8px;
    margin-bottom: 12px;
  }

  .node-label {
    font-weight: 600;
    font-size: 14px;
    color: #333;
    margin-bottom: 2px;
  }

  .node-type {
    font-size: 11px;
    color: #666;
    text-transform: uppercase;
    letter-spacing: 0.5px;
  }

  .pins-container {
    display: flex;
    flex-direction: column;
    gap: 8px;
  }

  .pin-item {
    border: 1px solid #f0f0f0;
    border-radius: 6px;
    padding: 8px;
    background: #fafafa;
    transition: all 0.2s ease;
    width: 100%;
    text-align: left;
    font-family: inherit;
    cursor: default;
  }

  .pin-item.clickable {
    cursor: pointer;
    background: #f8f9fa;
  }

  .pin-item.clickable:hover {
    background: #e9ecef;
    border-color: #007bff;
    transform: translateY(-1px);
    box-shadow: 0 2px 4px rgba(0, 123, 255, 0.1);
  }

  .pin-item:disabled {
    cursor: default;
    opacity: 0.7;
  }

  .pin-item.loading {
    background: #fff3cd;
    border-color: #ffc107;
  }

  .pin-header {
    display: flex;
    align-items: center;
    gap: 6px;
    margin-bottom: 4px;
  }

  .pin-icon {
    font-size: 14px;
  }

  .pin-label {
    font-weight: 500;
    font-size: 12px;
    color: #333;
    flex: 1;
  }

  .pin-count {
    font-size: 11px;
    color: #666;
    background: #e9ecef;
    padding: 1px 4px;
    border-radius: 3px;
  }

  .pin-summary {
    font-size: 11px;
    color: #666;
    margin-bottom: 4px;
    line-height: 1.3;
  }

  .pin-action {
    font-size: 10px;
    color: #007bff;
    font-weight: 500;
    text-align: right;
  }

  .pin-item.loading .pin-action {
    color: #856404;
  }
</style> 