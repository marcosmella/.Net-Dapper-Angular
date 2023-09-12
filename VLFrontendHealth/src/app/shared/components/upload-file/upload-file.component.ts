import { Component, OnInit, ViewChild, OnDestroy } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";

import { ElipsisAction, IconTypes } from "./../../../components/elipsis-grid/elipsis-grid.component";
import { DragDropFileComponent } from "../drag-drop-file/drag-drop-file.component";
import { MenuService } from "../../../services/menu.service";

@Component({
	selector: "app-upload-file",
	templateUrl: "./upload-file.component.html",
	styleUrls: ["./upload-file.component.scss"]
})
export class UploadFileComponent implements OnInit, OnDestroy {
	public acceptFormats: string[] = [".png", ".jpg", ".pdf", ".jpeg"];
	public entityId: number;
	public fileTypeId: number;
	public fileType: string;
	public active: boolean;
	public entityTypeId: number;
	public formUploadFile: FormGroup;
	@ViewChild("dragAndDropFile", { static: false })
	public dragAndDropFile: DragDropFileComponent;
	public ellipsisActions = new Array<ElipsisAction>();

	constructor(private router: Router, private fb: FormBuilder, private menuService: MenuService) {
		// ESTOS VALORES LOS SETEA QUIEN CREA ESTE COMPONENTE PADRE POR ESO ESTAN FIJOS AHORA SOLO PARA PRUEBAS
		// ES UNA SIMULACION
		this.entityId = 36330;
		this.fileTypeId = 10001;
		this.fileType = "file";
		this.active = false;
		this.entityTypeId = 1;
		this.setform();
	}

	ngOnInit(): void {
		this.menuService.hideMenu();
		this.ellipsisActions.push(
			{
				action: () => {
					this.dragAndDropFile.openViewer();
				},
				icon: IconTypes.view,
				description: "view"
			},
			{
				action: () => {
					this.dragAndDropFile.download();
				},
				icon: IconTypes.download,
				description: "download"
			},
			{
				action: () => {
					this.dragAndDropFile.delete();
				},
				icon: IconTypes.delete,
				description: "delete"
			}
		);
	}

	ngOnDestroy(): void {
		this.menuService.showMenu();
	}

	setform(): void {
		this.formUploadFile = this.fb.group({
			filename: [null, [Validators.required]]
		});
	}

	cancel(): void {
		this.router.navigateByUrl("../");
	}

	setFilename(event: Event): void {
		return this.formUploadFile.get("filename").setValue(event);
	}

	save(): void {
		if (!this.formUploadFile.valid) {
			this.formUploadFile.markAllAsTouched();
			return;
		}

		this.dragAndDropFile.save().then(() => {});
	}
}
