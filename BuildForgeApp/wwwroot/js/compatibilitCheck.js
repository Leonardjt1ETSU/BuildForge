document.addEventListener("DOMContentLoaded", () => {
    const button = document.getElementById("checkCompatibilityBtn");
    const resultBox = document.getElementById("compatibilityResult");

    if (!button || !resultBox) {
        return;
    }

    button.addEventListener("click", async () => {
        const buildId = button.dataset.buildId;

        const response = await fetch(`/api/BuildCompatibilityApi/${buildId}`);

        if (!response.ok) {
            resultBox.innerHTML = `<div class="alert alert-danger">Could not check compatibility.</div>`;
            return;
        }

        const result = await response.json();

        if (result.isCompatible) {
            resultBox.innerHTML = `<div class="alert alert-success">This build is compatible.</div>`;
        } else {
            const warningList = result.warnings.map(w => `<li>${w}</li>`).join("");

            resultBox.innerHTML = `
                <div class="alert alert-warning">
                    <strong>Compatibility Issues:</strong>
                    <ul class="mb-0">${warningList}</ul>
                </div>
            `;
        }
    });
});