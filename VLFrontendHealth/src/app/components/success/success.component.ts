import { Component, OnInit, OnDestroy } from "@angular/core";
import { Router } from "@angular/router";
import { MenuService } from "src/app/services/menu.service";

import { EnumFeedback } from "../../models/enumFeedback.model";
import { SuccessService } from "./../../services/success.service";

@Component({
	selector: "app-success",
	templateUrl: "./success.component.html",
	styleUrls: ["./success.component.scss"]
})
export class SuccessComponent implements OnInit, OnDestroy {
	constructor(public successServ: SuccessService, private router: Router, private menuServ: MenuService) {}

	ngOnInit(): void {
		this.menuServ.hideMenu();
		this.successServ.data.imageFeedBack = this.successServ.data.imageFeedBack ? this.successServ.data.imageFeedBack : EnumFeedback.done;
	}

	ngOnDestroy(): void {
		this.menuServ.showMenu();
	}

	actionContinue(): void {
		const route = this.successServ.data.routeContinue;
		this.clear();
		this.router.navigateByUrl(route);
	}

	finish(): void {
		const route = this.successServ.data.routeFinish;
		this.clear();
		this.router.navigateByUrl(route);
	}

	detail(): void {
		const route = this.successServ.data.routeDetail;
		this.clear();
		this.router.navigateByUrl(route);
	}

	executeAction(): void {
		this.successServ.data.optionalAction();
	}

	private clear(): void {
		this.successServ.data.imageFeedBack = null;
		this.successServ.data.actionContinue = null;
		this.successServ.data.routeContinue = null;
		this.successServ.data.actionDetail = null;
		this.successServ.data.routeDetail = null;
		this.successServ.data.actionFinish = null;
		this.successServ.data.routeFinish = null;
		this.successServ.data.description = null;
		this.successServ.data.actionFinalized = null;
		this.successServ.data.optionalLastText = null;
		this.successServ.data.optionalActionText = null;
		this.successServ.data.optionalAction = null;
	}
}
