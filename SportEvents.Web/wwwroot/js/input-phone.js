document.addEventListener("DOMContentLoaded", function () {
    const inputs = document.querySelectorAll(".phone-input");

    inputs.forEach(input => {
        if (input.dataset.intlInitialized === "true") {
            return;
        }

        const iti = window.intlTelInput(input, {
            initialCountry: "auto",
            nationalMode: false,
            formatOnDisplay: true,
            strictMode: true,
            geoIpLookup: function (callback) {
                fetch("https://ipapi.co/json")
                    .then(res => res.json())
                    .then(data => callback(data.country_code))
                    .catch(() => callback("ru"));
            },
            utilsScript: "https://cdn.jsdelivr.net/npm/intl-tel-input@23.0.11/build/js/utils.js"
        });

        input.dataset.intlInitialized = "true";

        input.addEventListener("input", function () {
            if (!this.value.startsWith("+")) {
                this.value = "+" + this.value.replace(/\+/g, "");
            }
        });

        input.addEventListener("keydown", function (e) {
            if (this.selectionStart === 0 && (e.key === "Backspace" || e.key === "Delete")) {
                e.preventDefault();
            }
        });

        const form = input.closest("form");
        if (form && !form.dataset.phoneValidationAttached) {
            form.addEventListener("submit", function (e) {
                const phoneInputs = form.querySelectorAll(".phone-input");

                for (const currentInput of phoneInputs) {
                    const wrapper = currentInput.closest(".phone-input-wrapper");
                    const errorMsg = wrapper?.querySelector(".text-danger");

                    const instance = window.intlTelInputGlobals.getInstance(currentInput);
                    if (!instance) {
                        continue;
                    }

                    if (!instance.isValidNumber()) {
                        if (errorMsg) {
                            const errorMap = [
                                "Неверный номер",
                                "Неверный код страны",
                                "Номер слишком короткий",
                                "Номер слишком длинный",
                                "Неверный номер"
                            ];

                            const errorCode = instance.getValidationError();
                            errorMsg.textContent = errorMap[errorCode] || "Неверный номер";
                        }

                        e.preventDefault();
                        return false;
                    }

                    if (errorMsg) {
                        errorMsg.textContent = "";
                    }

                    currentInput.value = instance.getNumber();
                }
            });

            form.dataset.phoneValidationAttached = "true";
        }
    });
});