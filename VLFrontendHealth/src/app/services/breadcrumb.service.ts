import { Injectable } from "@angular/core";
import { Subject } from "rxjs/internal/Subject";
import { Observable } from "rxjs/internal/Observable";

@Injectable({
	providedIn: "root"
})
export class BreadCrumbService {
	private subject$ = new Subject<any>();
	constructor() {}

	showBreadCrumb(breadCrumb: Array<BreadCrumb>): void {
		setTimeout(() => {
			this.subject$.next(breadCrumb);
		});
	}

	getBreadCrumb(): Observable<any> {
		return this.subject$.asObservable();
	}
}

export class BreadCrumb {
	label: string;
	path: string;
}
