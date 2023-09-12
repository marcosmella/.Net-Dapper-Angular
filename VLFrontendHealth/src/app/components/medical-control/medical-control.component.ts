import { Component, OnInit, OnDestroy } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { first } from "rxjs/operators";

import { MenuService } from "../../services/menu.service";

@Component({
	selector: "app-medical-control",
	templateUrl: "./medical-control.component.html",
	styleUrls: ["./medical-control.component.scss"]
})
export class MedicalControlComponent implements OnInit, OnDestroy {
	private fromMedicalHistory = false;
	private medicalHistoryIdEmployee: number;

	constructor(private router: Router, private menuService: MenuService, private route: ActivatedRoute) {}

	ngOnInit(): void {
		this.menuService.hideMenu();
		this.getParams();
	}

	ngOnDestroy(): void {
		this.menuService.showMenu();
	}

	getParams(): void {
		this.route.queryParamMap.pipe(first()).subscribe((params) => {
			this.medicalHistoryIdEmployee = Number(params.get("medicalHistoryIdEmployee"));
			if (this.medicalHistoryIdEmployee) {
				this.fromMedicalHistory = true;
			}
		});
	}

	createFromAbsenceRequested(): void {
		const params = this.medicalHistoryIdEmployee
			? { fromAbsenceRequested: true, medicalHistoryIdEmployee: this.medicalHistoryIdEmployee }
			: { fromAbsenceRequested: true };
		this.router.navigate(["healthApp", "medical-control", "create", "action", "absence"], { queryParams: params });
	}

	create(): void {
		this.medicalHistoryIdEmployee
			? this.router.navigateByUrl(`/healthApp/medical-control/create?medicalHistoryIdEmployee=${this.medicalHistoryIdEmployee}`)
			: this.router.navigateByUrl(`/healthApp/medical-control/create/`);
	}

	goBack(): void {
		let url = "/healthApp/medical-controls";
		if (this.fromMedicalHistory) {
			url = `/healthApp/medical-history/modify/${this.medicalHistoryIdEmployee}`;
		}
		this.router.navigateByUrl(url);
	}
}
