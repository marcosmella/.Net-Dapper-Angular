import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { AppConfigService } from "./app.config.service";
import { EmployeeAttributes } from "./../models/employee-attributes.model";

@Injectable({
	providedIn: "root"
})
export class EmployeeAttributeService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Organization/api/`;
	}

	public getEmployeeAttributes(idEmployee: number): Observable<EmployeeAttributes> {
		return this._http.get<EmployeeAttributes>(`${this._url}employee-attributes/GetEmployeeAttributes/${idEmployee}`);
	}
}
