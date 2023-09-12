import { Component, OnInit, ViewChild, Input } from "@angular/core";
import { MatSort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";

import { LoadingSpinnerComponent } from "./../../../../components/loading-spinner/loading-spinner.component";
import { WorkElementDeliveryStatus, WorkElementDeliveryRow } from "./../../../../models/employee-work-element.model";
import { UserPreference } from "./../../../../models/user-preference.model";
import { AlertService } from "./../../../../services/alert.service";
import { WorkElementsDeliveryFunctionsService } from "./../../../../services/work-elements-delivery-functions.service";

@Component({
	selector: "app-work-element-list",
	templateUrl: "./work-element-list.component.html",
	styleUrls: ["./work-element-list.component.scss"]
})
export class WorkElementListComponent implements OnInit {
	@ViewChild("spinnerList", { static: true })
	public spinner: LoadingSpinnerComponent;

	@ViewChild(MatSort, { static: true })
	public sort: MatSort;

	@Input() set idEmployee(id: number) {
		if (id) {
			this.getElementsByEmployee(id);
		}
	}

	public elementsDataSource: MatTableDataSource<any>;
	public elementsDisplayedColumns: string[] = ["date", "categoryDescription", "elementDescription", "amount", "signed", "status"];
	public get workElementDeliveryStatus(): typeof WorkElementDeliveryStatus {
		return WorkElementDeliveryStatus;
	}
	constructor(
		public userPreference: UserPreference,
		private workElementsDeliveryFunctionService: WorkElementsDeliveryFunctionsService,
		private alertService: AlertService
	) {
		this.elementsDataSource = new MatTableDataSource<WorkElementDeliveryRow>();
	}

	ngOnInit(): void {}

	getElementsByEmployee(idEmployee: number): void {
		this.spinner.show();

		this.workElementsDeliveryFunctionService
			.getWorkElementDeliveryByEmployee(idEmployee)
			.subscribe(
				(elements) => {
					this.elementsDataSource.data = elements;
					this.elementsDataSource.sort = this.sort;
					this.spinner.hide();
				},
				(error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "elementsError");
					}
					this.elementsDataSource.data = [];
					this.spinner.hide();
				}
			)

			.add(() => {
				this.spinner.hide();
			});
	}
}
