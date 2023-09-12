import { Component, AfterViewInit, OnInit } from "@angular/core";
import { Title } from "@angular/platform-browser";

import { BreadCrumbService } from "./../../services/breadcrumb.service";
import { TranslatePipe } from "./../../pipes/translate.pipe";

@Component({
	selector: "app-health-dashboard",
	templateUrl: "./health-dashboard.component.html",
	styleUrls: ["./health-dashboard.component.scss"]
})
export class HealthDashboardComponent implements AfterViewInit, OnInit {
	constructor(private title: Title, private breadCrumb: BreadCrumbService, private translatePipe: TranslatePipe) {
		this.title.setTitle(`${this.translatePipe.transform("health")} - Visma`);
	}
	ngAfterViewInit(): void {
		this.breadCrumb.showBreadCrumb([{ label: "hrCore", path: "/hrcore" }]);
	}

	ngOnInit(): void {}
}
