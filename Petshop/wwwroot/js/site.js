// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('input', function (event) {
	var target = event.target;

	if (!target || !target.matches('[data-cpf-input="true"]')) {
		return;
	}

	var digitsOnly = target.value.replace(/\D/g, '').slice(0, 11);
	if (target.value !== digitsOnly) {
		target.value = digitsOnly;
	}
});
