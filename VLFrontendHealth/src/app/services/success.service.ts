import { Injectable } from "@angular/core";

import { EnumFeedback } from "../models/enumFeedback.model";

@Injectable({
	providedIn: "root"
})
export class SuccessService {
	// tslint:disable-next-line: no-use-before-declare
	public data = new SuccessSharedData();

	constructor() {}

	clear(): void {
		// tslint:disable-next-line: no-use-before-declare
		this.data = new SuccessSharedData();
	}
}

class SuccessSharedData {
	title = "success";
	imageFeedBack: EnumFeedback = EnumFeedback.done;
	actionFinalized: string;
	description: string;
	actionContinue: string;
	actionFinish: string;
	routeContinue: string;
	routeFinish: string;
	actionDetail: string;
	routeDetail: string;
	optionalLastText: string;
	optionalActionText: string;
	optionalAction: () => void;
}
