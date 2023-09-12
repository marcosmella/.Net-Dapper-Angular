import { Renderer2, ElementRef, Input, OnDestroy } from "@angular/core";
import { Directive } from "@angular/core";
import { Subject } from "rxjs/internal/Subject";

import { FileManagementService } from "../services/file-management.service";

@Directive({
	// tslint:disable-next-line: directive-selector
	selector: "[appFileManagement]"
})
export class FileManagementDirective implements OnDestroy {
	@Input() set source(value: string) {
		this._source = value;
		this.getImage();
	}

	token: any = { cancel: {} };
	protected ngUnsubscribe$: Subject<void> = new Subject<void>();
	private _source: string;

	constructor(private renderer: Renderer2, private element: ElementRef, private fileManagment: FileManagementService) {}

	getImage(): void {
		const imageElement = this.renderer.selectRootElement(this.element.nativeElement, true);
		this.fileManagment.viewImage(this._source, this.token).then((image) => (imageElement.src = image));
	}
	ngOnDestroy(): void {
		if (typeof this.token.cancel === "function") {
			this.token.cancel();
		}
	}
}
