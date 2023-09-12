import { Component, OnInit, OnDestroy } from "@angular/core";
import { Router } from "@angular/router";
import { Subscription } from "rxjs/internal/Subscription";

import { BreadCrumbService, BreadCrumb } from "../../services/breadcrumb.service";

@Component({
	selector: "app-breadcrumb",
	templateUrl: "./breadcrumb.component.html",
	styleUrls: ["./breadcrumb.component.scss"]
})
export class BreadCrumbComponent implements OnInit, OnDestroy {
	public breadcrumbs: Array<BreadCrumb>;
	public subscription: Subscription;
	public previouPage: number;
	constructor(private router: Router, private breadCrumb: BreadCrumbService) {}

	ngOnInit(): void {
		this.subscription = this.breadCrumb.getBreadCrumb().subscribe((breadcrumb: Array<BreadCrumb>) => {
			this.breadcrumbs = breadcrumb;
		});
	}

	goToUrl(url: string): void {
		this.router.navigateByUrl(url);
	}

	goToPreviousPage(): void {
		this.goToUrl(this.breadcrumbs[this.breadcrumbs.length - 1].path);
	}

	ngOnDestroy(): void {
		this.subscription.unsubscribe();
	}
}
