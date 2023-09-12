import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";
import { Operation } from "fast-json-patch";

import { AppConfigService } from "./app.config.service";
import { Structure } from "../models/structure.model";
import { CodeType } from "../models/code-type.model";

@Injectable({
	providedIn: "root"
})
export class StructureService {
	private _url: string;
	private lastStructureCreated: Structure;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Organization/api/`;
	}

	public getCodeTypes(): Observable<CodeType[]> {
		return this._http.get<CodeType[]>(`${this._url}code-types`);
	}

	public get(idType: number): Observable<any> {
		return this._http.get(`${this._url}structures/${idType}/structures`);
	}

	public getId(id: number): Observable<any> {
		return this._http.get(`${this._url}structures/${id}`);
	}

	public getStructuresSelect(idType: number): Observable<any> {
		return this._http.get(`${this._url}structures/GetStructuresSelect/${idType}`);
	}

	public create(structure: Structure): Observable<any> {
		return this._http.post(`${this._url}structures`, structure);
	}

	public getLastStructureCreated(): Structure {
		return this.lastStructureCreated;
	}
	public setLastStructureCreated(structure: Structure): void {
		this.lastStructureCreated = structure;
	}

	public update(structure: Structure): Observable<any> {
		return this._http.put(`${this._url}structures`, structure);
	}

	public patch(id: number, operations: Operation[]): Observable<any> {
		return this._http.patch(`${this._url}structures/${id}`, operations);
	}
}
