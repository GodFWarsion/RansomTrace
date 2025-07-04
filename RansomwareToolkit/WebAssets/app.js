let playbookData = null;

mermaid.initialize({
  startOnLoad: true,
  securityLevel: 'loose'
});

// Fetch the playbook data
fetch('playbook.json')
  .then(response => response.json())
  .then(data => {
    playbookData = data;
    const mermaidText = buildMermaid(data);
    document.getElementById('mermaid').innerHTML = mermaidText;
    mermaid.init(undefined, '#mermaid');
  });

// Function to build the Mermaid diagram
function buildMermaid(data) {
  let chart = 'graph TD;\n';

  // Add each phase to the diagram
  data.phases.forEach(phase => {
    chart += `${phase.id}["${phase.title}"];\n`;
  });

  // Add links between phases
  data.links.forEach(link => {
    chart += `${link.from} --> ${link.to};\n`;
  });

  // Add click events to nodes
  data.phases.forEach(phase => {
    chart += `click ${phase.id} call showDetails("${phase.id}")\n`;
  });

  return chart;
}

// Function to show phase details
window.showDetails = function (phaseId) {
  // Find the phase by ID
  const phase = playbookData.phases.find(p => p.id === phaseId);
  
  if (phase) {
    // Get the elements where we will show the details
    const titleEl = document.getElementById('phase-title');
    const descEl = document.getElementById('phase-desc');
    const contentBox = document.querySelector('.content');

    // Update the title and description
    titleEl.textContent = phase.title;
    descEl.innerHTML = `
      <strong>Roles:</strong> ${phase.roles.join(', ')}<br/>
      <strong>Actions:</strong> ${phase.actions.join('<br/>')}
    `;

    // Trigger slide-in animation by toggling the 'active' class
    contentBox.classList.remove('active');
    void contentBox.offsetWidth; // Trigger reflow to restart animation
    contentBox.classList.add('active');
  }
};
