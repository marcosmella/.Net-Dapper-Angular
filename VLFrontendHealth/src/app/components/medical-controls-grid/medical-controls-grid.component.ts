import { AfterViewInit, Component, OnInit, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import * as moment from "moment";

import { BreadCrumbService } from "./../../services/breadcrumb.service";
import { UserPreference } from "./../../models/user-preference.model";
import { TranslatePipe } from "./../../pipes/translate.pipe";
import { MedicalControlFilterComponent } from "./medical-control-filter/medical-control-filter.component";
import { MedicalControlsListComponent } from "./medical-controls-list/medical-controls-list.component";

@Component({
	selector: "app-medical-controls-grid",
	templateUrl: "./medical-controls-grid.component.html",
	styleUrls: ["./medical-controls-grid.component.scss"]
})
export class MedicalControlsGridComponent implements OnInit, AfterViewInit {
	public quantityMedicalControls: number;
	public filtered = [];
	public filter = true;

	@ViewChild("medicalControlList", { static: false })
	public medicalControlList: MedicalControlsListComponent;

	@ViewChild("medicalControlFilter", { static: false })
	public medicalControlFilter: MedicalControlFilterComponent;

	constructor(
		private router: Router,
		private translatePipe: TranslatePipe,
		private breadCrumb: BreadCrumbService,
		public userPreference: UserPreference
	) {}

	ngOnInit(): void {}

	ngAfterViewInit(): void {
		this.initBreadCrumb();
		this.clearFilter();
		this.applyFilter();
		this.getChipFilterParam();
	}

	initBreadCrumb(): void {
		this.breadCrumb.showBreadCrumb([
			{ label: "hrCore", path: "/hrcore" },
			{ label: "health", path: "/healthApp" }
		]);
	}

	applyFilter(): void {
		this.medicalControlList.medicalControlsFilter = this.medicalControlFilter.medicalControlsFilter;
		this.medicalControlList.getMedicalControlByFilter();
	}

	onApplyFilter(): void {
		this.applyFilter();

		this.getChipFilterParam();
	}

	clearFilter(): void {
		this.medicalControlFilter.clearFilter();
	}
	clickClearFilter(): void {
		this.clearFilter();

		this.getChipFilterParam();
	}

	updateFilter(): void {
		this.medicalControlFilter.updateFilter();
	}

	clearProperty(name: string): void {
		this.medicalControlFilter.clearProperty(name);
	}

	getChipFilterParam(): void {
		let elementValue: string;
		let defaultDescription = true;

		this.filtered = [];
		Object.entries(this.medicalControlFilter.medicalControlsFilter).forEach((element) => {
			elementValue = element[1];
			if (element[0] === "lastName" || element[0] === "name" || element[0] === "fileNumber") {
				elementValue = this.translatePipe.transform(element[0]);
				defaultDescription = false;
			} else {
				defaultDescription = true;
			}

			if (element[0] === "idMedicalControlAction") {
				elementValue = `${this.translatePipe.transform("actionType")}: ${
					this.medicalControlFilter.medicalControlsFilter.actionDescription
				}`;
				defaultDescription = false;
			} else {
				defaultDescription = true;
			}

			if (element[0] === "idStructureType") {
				elementValue = `${this.translatePipe.transform("structureType")}: ${
					this.medicalControlFilter.medicalControlsFilter.structureTypeDescription
				}`;
				defaultDescription = false;
			}

			if (element[0] === "idStructure") {
				elementValue = `${this.translatePipe.transform("Attribute")}: ${
					this.medicalControlFilter.medicalControlsFilter.structureDescription
				}`;
				defaultDescription = false;
			}

			if (element[0] === "medicalControlRange") {
				const dateFromDescription = this.translatePipe.transform("from");
				const medicalControlRange = this.medicalControlFilter.medicalControlsFilter.medicalControlRange;

				if (medicalControlRange.start) {
					const startDate = moment(this.medicalControlFilter.medicalControlsFilter.medicalControlRange.start);
					const date = startDate.format(this.userPreference.dateFormat.format.toUpperCase());
					elementValue = `${dateFromDescription}: ${date}`;
					defaultDescription = false;
				}

				const dateToDescription = this.translatePipe.transform("to");
				if (medicalControlRange.end) {
					const endDate = moment(this.medicalControlFilter.medicalControlsFilter.medicalControlRange.end);
					const date = endDate.format(this.userPreference.dateFormat.format.toUpperCase());
					elementValue += elementValue !== "" ? " - " : `${date}: `;
					elementValue += `${dateToDescription} ${date}`;
					defaultDescription = false;
				}
				if (!defaultDescription) {
					this.filtered.push({ name: element[0], text: elementValue, removable: true });
				}
			}

			if (defaultDescription) {
				elementValue = this.translatePipe.transform(element[0]);
				elementValue += `: ${element[1]}`;
			}

			if (
				element[1] &&
				element[0] !== "medicalControlRange" &&
				element[0] !== "structure" &&
				element[0] !== "idControlType" &&
				element[0] !== "idAbsenceType" &&
				element[0] !== "structureTypeDescription" &&
				element[0] !== "structureDescription" &&
				element[0] !== "actionDescription" &&
				element[0] !== "pageSize" &&
				element[0] !== "page" &&
				element[0] !== "orderBy"
			) {
				this.filtered.push({ name: element[0], text: elementValue.toUpperCase(), removable: true });
			}
		});
	}

	removeChip(name: string): void {
		this.filtered = Object.assign(
			this.filtered.filter((p) => p.name !== name),
			[]
		);
		this.clearProperty(name);
		this.updateFilter();
		this.applyFilter();
	}

	import(): void {
		this.setReturnUrl();
		this.router.navigateByUrl(`/massiveOperationApp/import/health`);
	}

	private setReturnUrl(): void {
		localStorage.setItem("returnUrl", this.router.url);
	}
}
