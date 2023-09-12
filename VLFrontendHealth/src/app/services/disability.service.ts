import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { AppConfigService } from "./app.config.service";
import { Select } from "../models/select.model";
import { PersonDisability } from "../models/personDisability";

@Injectable({
	providedIn: "root"
})
export class DisabilityService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Person/api/`;
	}

	public get(): Observable<Select[]> {
		return this._http.get<Select[]>(`${this._url}disabilities`);
	}

	public getById(idPerson: number): Observable<Select> {
		return this._http.get<Select>(`${this._url}disabilities/${idPerson}`);
	}

	public post(person: PersonDisability): Observable<any> {
		return this._http.post(`${this._url}physical-persons/${person.idPerson}/disabilities`, person);
	}

	public put(person: PersonDisability): Observable<any> {
		return this._http.put(`${this._url}physical-persons/${person.idPerson}/disabilities`, person);
	}

	public delete(person: PersonDisability): Observable<any> {
		return this._http.delete(`${this._url}physical-persons/${person.idPerson}/disabilities`);
	}
}
