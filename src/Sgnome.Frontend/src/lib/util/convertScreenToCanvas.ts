/**
   * Converts screen coordinates to canvas coordinates
   * @param clientX - Screen X coordinate
   * @param clientY - Screen Y coordinate
   * @returns Canvas coordinates { x, y }
   */
export function convertScreenToCanvas(clientX: number, clientY: number): { x: number; y: number } {
    // Get the flow container element
    const flowElement = document.querySelector(".svelte-flow") as HTMLElement;
    if (!flowElement) {
      console.warn("Flow element not found, using screen coordinates");
      return { x: clientX, y: clientY };
    }

    const rect = flowElement.getBoundingClientRect();
    
    // Get the viewport transform from the flow element
    const viewportElement = flowElement.querySelector(".svelte-flow__viewport") as HTMLElement;
    if (viewportElement) {
      const transform = window.getComputedStyle(viewportElement).transform;
      const matrix = new DOMMatrix(transform);
      
      // Convert screen coordinates to canvas coordinates
      const canvasX = (clientX - rect.left - matrix.m41) / matrix.m11;
      const canvasY = (clientY - rect.top - matrix.m42) / matrix.m22;
      
      console.log("Converted to canvas coordinates:", { canvasX, canvasY });
      return { x: canvasX, y: canvasY };
    } else {
      // Fallback to simple relative positioning
      const canvasX = clientX - rect.left;
      const canvasY = clientY - rect.top;
      
      console.log("Using fallback coordinates:", { canvasX, canvasY });
      return { x: canvasX, y: canvasY };
    }
  }