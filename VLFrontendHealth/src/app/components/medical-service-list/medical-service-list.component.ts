import { AfterViewInit, Component, OnInit, ViewChild } from "@angular/core";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";
import { Router } from "@angular/router";

import { AlertService } from "./../../services/alert.service";
import { BreadCrumbService } from "./../../services/breadcrumb.service";
import { ExcelService } from "./../../services/excel.services";
import { ModalService } from "./../../services/modal.service";
import { SnackBarService } from "./../../services/snack-bar.service";
import { ElipsisAction, IconTypes } from "../elipsis-grid/elipsis-grid.component";
import { LoadingSpinnerComponent } from "../loading-spinner/loading-spinner.component";
import { MedicalService } from "./../../models/medical-service";
import { MedicalServiceService } from "./../../services/medical-service.service";

@Component({
	selector: "app-medical-service-list",
	templateUrl: "./medical-service-list.component.html",
	styleUrls: ["./medical-service-list.component.scss"]
})
export class MedicalServiceListComponent implements OnInit, AfterViewInit {
	@ViewChild("spinner", { static: false })
	public spinner: LoadingSpinnerComponent;
	@ViewChild(MatSort, { static: false })
	public sort: MatSort;
	@ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
	public medicalserviceDisplayedColumns: string[] = ["id", "company", "phone", "options"];
	public medicalserviceDataSource: MatTableDataSource<MedicalService>;
	public actions: ElipsisAction[];
	public recordsCount: number;

	constructor(
		private medicalserviceService: MedicalServiceService,
		private router: Router,
		private alertService: AlertService,
		private modalService: ModalService,
		private snackBarService: SnackBarService,
		private breadCrumb: BreadCrumbService,
		private excelService: ExcelService
	) {
		this.medicalserviceDataSource = new MatTableDataSource<MedicalService>();
		this.actions = new Array<ElipsisAction>();
		this.actions.push(
			{
				action: (element) => this.edit(element.id),
				icon: IconTypes.edit
			},
			{
				action: (element) => this.delete(element.id),
				icon: IconTypes.delete
			}
		);
	}

	ngOnInit(): void {}

	ngAfterViewInit(): void {
		this.getMedicalServices();
		this.breadCrumb.showBreadCrumb([
			{ label: "hrCore", path: "/hrcore" },
			{ label: "health", path: "/healthApp" }
		]);
	}

	getMedicalServices(): void {
		this.spinner.show();
		this.medicalserviceService
			.get()
			.subscribe(
				(medicalservice) => {
					this.medicalserviceDataSource.data = medicalservice;
					this.medicalserviceDataSource.paginator = this.paginator;
					this.medicalserviceDataSource.sort = this.sort;
					this.recordsCount = this.medicalserviceDataSource.data.length;
				},
				(error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "ErrorMedicalServiceList");
					}
				}
			)
			.add(() => {
				this.spinner.hide();
			});
	}

	create(): void {
		this.router.navigateByUrl(`/healthApp/medical-service/create`);
	}

	edit(id: number): void {
		this.router.navigateByUrl(`/healthApp/medical-service/modify/${id}`);
	}

	delete(id: number): void {
		this.modalService
			.openDialog({
				title: "atention",
				message: "areYouSureForDelete",
				noButtonMessage: "cancel",
				okButtonMessage: "yesIAmSure"
			})
			.subscribe((accept) => {
				if (accept) {
					this.spinner.show();
					this.medicalserviceService
						.delete(id)
						.subscribe(
							() => {
								this.snackBarService.openSnackBar({
									message: "deletedSuccessfully",
									icon: true,
									action: null,
									secondsDuration: 5
								});
								this.getMedicalServices();
							},
							(error) => {
								this.alertService.error(error.error.Error, "ErrorMedicalServiceList");
							}
						)
						.add(() => {
							this.spinner.hide();
						});
				}
			});
	}

	applyFilter(filterValue: string): void {
		this.medicalserviceDataSource.filter = filterValue.trim().toLowerCase();
		this.recordsCount = this.medicalserviceDataSource._filterData(this.medicalserviceDataSource.data).length;
	}

	exportAsXLSX(): void {
		let filteredMedicalServices = [];
		const filtered = this.medicalserviceDataSource._orderData(this.medicalserviceDataSource._filterData(this.medicalserviceDataSource.data));

		filteredMedicalServices = filtered.map((value) => ({
			id: value.id,
			company: value.company,
			phone: value.phone
		}));
		this.excelService.exportAsExcelFile(filteredMedicalServices, "MedicalServicees");
	}
}
