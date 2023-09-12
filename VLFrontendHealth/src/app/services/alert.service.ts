import { Injectable } from "@angular/core";
import { Subject } from "rxjs/internal/Subject";
import { Observable } from "rxjs/internal/Observable";

@Injectable({
	providedIn: "root"
})
export class AlertService {
	private subject$ = new Subject<Message>();

	private clear$ = new Subject<Array<string>>();

	constructor() {}

	warning(message: Array<string>, alertId: string = null): void {
		this.subject$.next({ id: alertId, type: "warning", list: message });
	}

	error(message: Array<string>, alertId: string = null): void {
		this.subject$.next({ id: alertId, type: "error", list: message });
	}

	info(message: Array<string>, alertId: string = null): void {
		this.subject$.next({ id: alertId, type: "info", list: message });
	}

	removeAlerts(alertsId: Array<string> = null): void {
		this.clear$.next(alertsId);
	}
	clear(): Observable<any> {
		return this.clear$.asObservable();
	}

	getMessage(): Observable<any> {
		return this.subject$.asObservable();
	}
}

export interface AlertId {
	id: Array<string>;
}
export interface Message {
	id: string;
	type: string;
	list: Array<any>;
}
