import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/internal/Observable";

import { EmployeeVaccine, EmployeeVaccines } from "../models/employee-vaccine.model";
import { AppConfigService } from "./app.config.service";

@Injectable({
	providedIn: "root"
})
export class EmployeeVaccinesService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Health/api/clinical-records`;
	}

	public getByEmployee(idEmployee: number): Observable<EmployeeVaccine[]> {
		return this._http.get<EmployeeVaccine[]>(`${this._url}/employees/${idEmployee}/vaccines`);
	}
	public put(employeeVaccines: EmployeeVaccines): Observable<any> {
		return this._http.put(`${this._url}/employees/vaccines`, employeeVaccines);
	}
}
