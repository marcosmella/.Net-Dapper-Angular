import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { AppConfigService } from "./app.config.service";
import { PersonalInformation } from "../models/personal-information.model";
import { Employee } from "../models/employee.model";

@Injectable({
	providedIn: "root"
})
export class PersonService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Person/api/`;
	}

	public getEmployeeById(idPerson: number): Observable<Employee> {
		return this._http.get<Employee>(`${this._url}employees/${idPerson}`);
	}

	public getById(id: number): Observable<PersonalInformation> {
		return this._http.get<PersonalInformation>(`${this._url}physical-persons/${id}`);
	}

	public getPersonById(id: number): Observable<PersonalInformation> {
		return this._http.get<PersonalInformation>(`${this._url}physical-persons/${id}`);
	}
}
