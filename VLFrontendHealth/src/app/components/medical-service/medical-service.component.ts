import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";

import { AlertService } from "./../../services/alert.service";
import { MenuService } from "./../../services/menu.service";
import { SnackBarService } from "./../../services/snack-bar.service";
import { SuccessService } from "./../../services/success.service";
import { LoadingSpinnerComponent } from "../loading-spinner/loading-spinner.component";
import { EnumFeedback } from "./../../models/enumFeedback.model";
import { MedicalService } from "./../../models/medical-service";
import { MedicalServiceService } from "./../../services/medical-service.service";
import { TranslatePipe } from "./../../pipes/translate.pipe";

@Component({
	selector: "app-medical-service",
	templateUrl: "./medical-service.component.html",
	styleUrls: ["./medical-service.component.scss"]
})
export class MedicalServiceComponent implements OnInit, AfterViewInit, OnDestroy {
	public formMedicalService: FormGroup;
	@ViewChild("spinner", { static: false })
	spinner: LoadingSpinnerComponent;
	public isUpdate = false;

	constructor(
		private fb: FormBuilder,
		private medicalServiceService: MedicalServiceService,
		private snackBarService: SnackBarService,
		private alertService: AlertService,
		private router: Router,
		private route: ActivatedRoute,
		private successServ: SuccessService,
		private menuServ: MenuService,
		private translatePipe: TranslatePipe
	) {
		this.formMedicalService = this.fb.group({
			id: [0],
			company: [null, [Validators.required, Validators.maxLength(50)]],
			phone: [null, [Validators.required, Validators.maxLength(20)]]
		});
	}

	ngOnInit(): void {
		this.menuServ.hideMenu();
	}

	ngAfterViewInit(): void {
		this.route.paramMap.subscribe((params) => {
			const idMedicalService = Number(params.get("id"));
			if (idMedicalService) {
				this.getMedicalServiceById(idMedicalService);
			}
		});
	}

	ngOnDestroy(): void {
		this.menuServ.showMenu();
	}

	getMedicalServiceById(idMedicalService: number): void {
		this.spinner.show();
		this.medicalServiceService
			.getById(idMedicalService)
			.subscribe(
				(data) => {
					this.isUpdate = true;
					this.formMedicalService.reset(data);
				},
				(error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "medicalServiceError");
					}
				}
			)
			.add(() => {
				this.spinner.hide();
			});
	}

	update(medicalService: MedicalService): void {
		this.medicalServiceService
			.put(medicalService)
			.subscribe(
				() => {
					this.snackBarService.openSnackBar({
						message: `modifiedSuccessfully`,
						icon: true,
						secondsDuration: 5,
						action: null
					});

					this.router.navigateByUrl("/healthApp/medical-services");
				},
				(error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "medicalServiceError");
					}
				}
			)
			.add(() => {
				this.spinner.hide();
			});
	}

	add(medicalService: MedicalService): void {
		this.medicalServiceService
			.post(medicalService)
			.subscribe(
				() => {
					this.feedbackOk();
				},
				(error) => {
					if (error.status !== 404) {
						this.alertService.error(error.error.Error, "medicalServiceError");
					}
				}
			)
			.add(() => this.spinner.hide());
	}

	submit(): void {
		if (!this.formMedicalService.valid) {
			this.formMedicalService.markAllAsTouched();
			return;
		}

		this.spinner.show();
		if (this.isUpdate) {
			this.update(this.formMedicalService.value);
		} else {
			this.add(this.formMedicalService.value);
		}
	}

	feedbackOk(): void {
		this.successServ.clear();
		const textDescription = this.translatePipe.transform("successMedicalServiceDescription");
		const textMedicalServiceDescription = `"${this.formMedicalService.value.company}"`;
		this.successServ.data.title = "success";
		this.successServ.data.imageFeedBack = EnumFeedback.done;
		this.successServ.data.actionContinue = null;
		this.successServ.data.routeContinue = null;
		this.successServ.data.actionFinish = "finish";
		this.successServ.data.routeFinish = "/healthApp/medical-services";
		this.successServ.data.description = `${textDescription} ${textMedicalServiceDescription}`;
		this.successServ.data.optionalLastText = "";
		this.successServ.data.optionalActionText = "";
		this.successServ.data.optionalAction = null;
		this.router.navigateByUrl("/healthApp/success");
	}
}
