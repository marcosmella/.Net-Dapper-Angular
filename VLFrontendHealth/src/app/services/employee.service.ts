import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { AppConfigService } from ".//app.config.service";
import { Employee } from "../models/employee.model";

@Injectable({
	providedIn: "root"
})
export class EmployeeService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Person/api/employees/`;
	}

	public get(idPerson: number): Observable<Employee> {
		return this._http.get<Employee>(`${this._url}/${idPerson}`);
	}
}
