import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/internal/Observable";

import { AppConfigService } from "./app.config.service";
import { Note } from "../models/notes.model";

@Injectable({
	providedIn: "root"
})
export class NoteService {
	private _url: string;

	constructor(private _http: HttpClient, private conf: AppConfigService) {
		this._url = `${this.conf.apiEndpoint}Person/api/employees/`;
	}

	public get(idPerson: number): Observable<Note[]> {
		return this._http.get<Note[]>(`${this._url}${idPerson}/notes`);
	}

	public post(note: Note): Observable<any> {
		return this._http.post(`${this._url}notes`, note);
	}

	public put(note: Note): Observable<any> {
		return this._http.put(`${this._url}notes`, note);
	}

	public delete(idNote: number, idPerson: number): Observable<any> {
		return this._http.delete(`${this._url}${idPerson}/notes/${idNote}`);
	}
}
