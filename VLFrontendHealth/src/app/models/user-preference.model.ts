import { Injectable } from "@angular/core";

@Injectable()
export class UserPreference {
	defaultPreference = 1;
	dateFormat = { id: this.defaultPreference, format: "dd/MM/yyyy" };
	numberFormat = { id: this.defaultPreference, thousandSeparator: ".", decimals: "2" };
	timeFormat = { id: this.defaultPreference, format: "HH:mm" };
	countryCodeFormat = { format: "es-ar" };

	constructor() {
		this.refresh();
	}

	public refresh(): void {
		const userPreference = JSON.parse(localStorage.getItem("userPreferences"));
		if (userPreference) {
			this.dateFormat = userPreference.dateFormat;
			this.numberFormat = userPreference.numberFormat;
			this.timeFormat = userPreference.timeFormat;
			this.countryCodeFormat = this.countryCodeFormat;
		} else {
			localStorage.setItem(
				"userPreferences",
				JSON.stringify({
					dateFormat: this.dateFormat,
					numberFormat: this.numberFormat,
					timeFormat: this.numberFormat,
					countryCodeFormat: this.countryCodeFormat
				})
			);
		}
	}
}
