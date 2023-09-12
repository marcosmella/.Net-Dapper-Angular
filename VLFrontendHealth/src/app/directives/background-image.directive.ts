import { AfterViewInit, Directive, ElementRef, Input, OnChanges, OnDestroy, Renderer2, SimpleChanges } from "@angular/core";
import { Subject } from "rxjs/internal/Subject";

import { FileManagementService } from "../services/file-management.service";

@Directive({
	// tslint:disable-next-line:directive-selector
	selector: "[backgroundImage]"
})
export class BackgroundImageDirective implements OnDestroy, AfterViewInit, OnChanges {
	@Input() set backgroundImage(value: string) {
		this.source = value;
	}
	token: any = { cancel: {} };
	protected ngUnsubscribe$: Subject<void> = new Subject<void>();
	private el: HTMLElement;
	private source: string;

	constructor(private renderer: Renderer2, private elRef: ElementRef, private fileManagment: FileManagementService) {
		this.el = this.elRef.nativeElement;
	}

	ngAfterViewInit(): void {
		this.setBackgroundImage();
	}

	ngOnChanges(changes: SimpleChanges): void {
		if (changes["backgroundImage"]) {
			this.setBackgroundImage();
		}
	}

	setBackgroundImage(): void {
		this.fileManagment
			.viewImage(this.source, this.token)
			.then(
				(image) => (
					(this.source = image),
					this.renderer.setStyle(this.el, "backgroundImage", `url(${this.source})`),
					this.renderer.setStyle(this.el, "background-repeat", "no-repeat"),
					this.renderer.setStyle(this.el, "background-size", "cover"),
					this.renderer.setStyle(this.el, "background-position", "center")
				)
			);
	}

	ngOnDestroy(): void {
		this.token.cancel();
	}
}
