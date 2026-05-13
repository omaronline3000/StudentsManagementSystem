// Admin Dashboard JavaScript Helpers

// Delete modal functions
function openDeleteModal(itemId, itemName) {
    const overlay = document.getElementById('deleteOverlay');
    if (overlay) {
        document.getElementById('deleteText').innerHTML =
            `Are you sure you want to delete <strong>${escapeHtml(itemName)}</strong>?<br>This action cannot be undone.`;
        overlay.classList.add('show');
        window.currentDeleteId = itemId;
    }
}

function closeDeleteModal() {
    const overlay = document.getElementById('deleteOverlay');
    if (overlay) {
        overlay.classList.remove('show');
    }
    window.currentDeleteId = null;
}

function confirmDelete() {
    if (window.currentDeleteId) {
        // This will be handled by individual views
        console.log('Delete ID:', window.currentDeleteId);
    }
}

// Utility function to escape HTML
function escapeHtml(text) {
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.replace(/[&<>"']/g, m => map[m]);
}

// Close delete modal when clicking outside
document.addEventListener('DOMContentLoaded', function () {
    const deleteOverlay = document.getElementById('deleteOverlay');
    if (deleteOverlay) {
        deleteOverlay.addEventListener('click', function (e) {
            if (e.target === this) {
                closeDeleteModal();
            }
        });
    }

    // Search functionality (generic)
    const searchInputs = document.querySelectorAll('[id*="searchInput"]');
    searchInputs.forEach(input => {
        input.addEventListener('keyup', function () {
            const searchTerm = this.value.toLowerCase();
            const rows = document.querySelectorAll('tbody tr');

            rows.forEach(row => {
                let match = false;
                const cells = row.querySelectorAll('td');

                cells.forEach(cell => {
                    if (cell.textContent.toLowerCase().includes(searchTerm)) {
                        match = true;
                    }
                });

                row.style.display = match ? '' : 'none';
            });
        });
    });

    // Form validation
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            // Basic client-side validation can be added here
            // Server-side validation is already in place
        });
    });
});

// Dismiss alerts after 5 seconds
window.addEventListener('load', function () {
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });
});
