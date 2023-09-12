import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { UserPreference } from "../models/user-preference.model";
import { AppConfigService } from "./app.config.service";
import { TimeFormatType } from "../models/enumTimeFormatType.model";

@Injectable({
	providedIn: "root"
})
export class UserPreferenceService {
	private _url: string;
	private _DEFAULT_FORMAT_DATE = "dd/MM/yyyy";
	private _DEFAULT_FORMAT_TIME = 24;

	constructor(private _http: HttpClient, private conf: AppConfigService, public _userPreference: UserPreference) {
		this.conf.loading().subscribe((data) => {
			if (!data) {
				this._url = `${this.conf.apiEndpoint}User/api`;
			}
		});
	}

	public getUserSettings(): Observable<any> {
		const userData = JSON.parse(localStorage.getItem("userData"));
		return new Observable((observer) => {
			this._http.get<any>(`${this._url}/user-setting/${userData.UserId}`).subscribe(
				(data) => {
					if (data) {
						localStorage.setItem("userPreferences", JSON.stringify(data));
						this._userPreference.refresh();
						observer.next(true);
					} else {
						observer.next(false);
					}
					observer.complete();
				},
				(error) => {
					observer.error(error);
				}
			);
		});
	}

	public getDateFormatApp(): any {
		const dateFormat = this._userPreference ? this._userPreference.dateFormat.format.toUpperCase() : this._DEFAULT_FORMAT_DATE;
		return dateFormat;
	}

	public getTimeFormatApp(): number {
		const format = TimeFormatType[this._userPreference.timeFormat.format]
			? TimeFormatType[this._userPreference.timeFormat.format]
			: this._DEFAULT_FORMAT_TIME;
		return format;
	}
}
